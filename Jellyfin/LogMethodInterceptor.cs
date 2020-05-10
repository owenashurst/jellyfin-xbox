using System;
using System.Linq.Expressions;
using System.Reflection;
using Jellyfin.Logging;
using MethodDecorator.Fody.Interfaces;

// TODO smurancsik dos it cause ExecutionEngineException?

//// See https://stackoverflow.com/a/39712347
//[module: LogMethod] // Atribute should be "registered" by adding as module or assembly custom attribute

//// Any attribute which provides OnEntry/OnExit/OnException with proper args
//namespace Jellyfin.Logging
//{
//    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Assembly |
//                    AttributeTargets.Module)]
//    public class LogMethodAttribute : Attribute, IMethodDecorator
//    {
//        private readonly NLog.Logger Logger = NLog.LogManager.GetLogger("LogMethod");

//        private MethodBase _method;

//        // instance, method and args can be captured here and stored in attribute instance fields
//        // for future usage in OnEntry/OnExit/OnException
//        public void Init(object instance, MethodBase method, object[] args)
//        {
//            try
//            {
//                _method = method;

//            }
//            catch (Exception)
//            {
//            }
//        }

//        public void OnEntry()
//        {
//            try
//            {
//                Logger.Debug("Entering into {0}", _method.Name);
//            }
//            catch (Exception)
//            {
//            }
//        }

//        public void OnExit()
//        {
//            try
//            {
//                Logger.Debug("Exiting into {0}", _method.Name);
//            }
//            catch (Exception)
//            {
//            }
//        }

//        public void OnException(Exception exception)
//        {
//            try
//            {
//                Logger.Debug(exception, "Exception {0}", _method.Name);
//            }
//            catch (Exception)
//            {
//            }
//        }
//    }
//}
