using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Zenject;
using ZenjectSignalCommands.Runtime.Internal.Attributes;
using ZenjectSignalCommands.Runtime.Main;

namespace ZenjectSignalCommands.Runtime.Internal.Binders
{
    public class CommandPool
    {
        private static readonly ConcurrentDictionary<Type, Dictionary<Type, Action<object, object>>> _fieldSettersCache = new();
        private readonly DiContainer _container;
        private readonly Dictionary<Type, ICommand> _commandPoolLookup = new();

        public CommandPool(DiContainer container)
        {
            _container = container;
        }
        
        public ICommand GetCommand<TSignal>(Type commandType, TSignal signal)
        {
            ICommand command;
            
            if (_commandPoolLookup.TryGetValue(commandType, out ICommand pooledCommand))
            {
                command =  ReInjectCommand(commandType, pooledCommand, signal);
            }
            else
            {
                command = CreateCommand(commandType, signal);
                _commandPoolLookup[commandType] = command;
            }
            
            return command;
        }

        private static ICommand ReInjectCommand<TSignal>(Type commandType, ICommand command, TSignal signal)
        {
            Type signalType = signal.GetType();
            
            return InjectCommand(commandType, signal, signalType, command);
        }
        
        private ICommand CreateCommand<TSignal>(Type commandType, TSignal signal)
        {
            Type signalType = signal.GetType();
            
            ICommand command = _container.Instantiate(commandType) as ICommand;

            return InjectCommand(commandType, signal, signalType, command);
        }

        private static ICommand InjectCommand<TSignal>(Type commandType, TSignal signal, Type signalType, ICommand command)
        {
            // Get or create field setters for the command type.
            Dictionary<Type, Action<object, object>> commandFieldSetters = GetOrCreateFieldSetters(commandType);

            // Iterate over signal fields and set corresponding command fields.
            foreach (var signalField in GetAllFields(signalType))
            {
                if (commandFieldSetters.TryGetValue(signalField.FieldType, out Action<object, object> commandFieldSetter))
                {
                    object signalValue = signalField.GetValue(signal);
                    commandFieldSetter(command, signalValue);
                }
            }

            return command;
        }

        private static Dictionary<Type, Action<object, object>> GetOrCreateFieldSetters(Type commandType)
        {
            return _fieldSettersCache.GetOrAdd(commandType, t =>
            {
                Dictionary<Type, Action<object, object>> setters = new();

                IEnumerable<FieldInfo> targetFields = GetAllFields(t).Where(x => x.GetCustomAttribute<CommandInjectAttribute>() != null);
                
                foreach (FieldInfo fieldInfo in targetFields)
                {
                    if (fieldInfo.IsInitOnly || fieldInfo.IsLiteral) 
                    {
                        throw new InvalidOperationException($"The CommandInjectAttribute cannot be applied to read-only fields. Field: {fieldInfo.Name} in Type: {commandType.FullName}");
                    }
                    
                    ParameterExpression target = Expression.Parameter(typeof(object), "target");
                    ParameterExpression value = Expression.Parameter(typeof(object), "value");
                    MemberExpression field = Expression.Field(Expression.Convert(target, t), fieldInfo);
                    BinaryExpression assign = Expression.Assign(field, Expression.Convert(value, fieldInfo.FieldType));

                    Action<object, object> setter = Expression.Lambda<Action<object, object>>(assign, target, value).Compile();
                    setters[fieldInfo.FieldType] = setter;
                }
                
                return setters;
            });
        }

        private static IEnumerable<FieldInfo> GetAllFields(Type type)
        {
            if (type == null)
            {
                return Enumerable.Empty<FieldInfo>();
            }

            BindingFlags flags = BindingFlags.Public |
                                 BindingFlags.NonPublic |
                                 BindingFlags.Instance |
                                 BindingFlags.DeclaredOnly;
            
            return type.GetFields(flags).Union(GetAllFields(type.BaseType));
        }
    }
}