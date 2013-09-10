using System;
using System.Collections.Generic;
using System.Reflection;

namespace W3Developments.Auditor
{
    internal class ChangeLoggerGlobal
    {
        private Object _MainObject { get; set; }
        public Object MainObject { get { return _MainObject; } }
        public IList<PropertyInfo> PropertyChain { get; set; }
        public IList<ChangeLog> Changes { get; set; }
        public ChangeLoggerGlobal(Object mainObject)
        {
            this._MainObject = mainObject;
            this.Changes = new List<ChangeLog>();
        }
    }
    /// <summary>
    /// ChangeLogger Class
    /// </summary>
    public class ChangeLogger
    {
        private Int64 Id { get; set; }
        private Object X { get; set; }
        private Object Y { get; set; }
        /// <summary>
        /// Indicates if any section of the audit failed
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// If not successful the exception details will be passed through here
        /// </summary>
        public Exception Exception { get; set; }
        private ChangeLoggerGlobal GlobalSettings { get; set; }
        /// <summary>
        /// Lists the differences between the two objects following auditing
        /// </summary>
        public IList<ChangeLog> Changes { get { return GlobalSettings.Changes; } }
        /// <summary>
        /// Create a new instance of ChangeLogger. The Objects must be of the same type
        /// </summary>
        /// <param name="id">The main object id that you want changes logged against</param>
        /// <param name="original">The original object to compare against</param>
        /// <param name="changed">The changed object to check for changes</param>
        public ChangeLogger(Int64 id, Object original, Object changed)
        {
            this.Success = true;
            if (original.GetType() != changed.GetType())
            {
                this.Success = false;
                this.Exception = new UnmatchedTypeException();
                return;
            }
            this.Id = id;
            this.X = original;
            this.Y = changed;
            this.GlobalSettings = new ChangeLoggerGlobal(original);
            this.GlobalSettings.PropertyChain = new List<PropertyInfo>();
        }
        private ChangeLogger(Int64 id, ChangeLoggerGlobal globalSettings, PropertyInfo parentProperty, Object original, Object changed)
        {
            this.Id = id;
            this.X = original;
            this.Y = changed;
            this.GlobalSettings = globalSettings;
            this.GlobalSettings.PropertyChain.Add(parentProperty);
        }

        /// <summary>
        /// Takes the two objects from the constructor and checks for differences between the object types decorated with the Audited attribute.
        /// Differences between the two objects are logged in the Changes List.
        /// </summary>
        public void Audit()
        {
            try
            {
                PropertyInfo[] propertyList = X.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
                foreach (PropertyInfo PI in propertyList)
                {
                    if (IsAudited(PI))
                    {
                        if (isOfSimpleType(PI.GetValue(X, null)))
                        {
                            Compare(new TypePropertyInfoPair(X.GetType(), PI));
                        }
                        else
                        {
                            ChangeLogger logger = new ChangeLogger(this.Id, this.GlobalSettings, PI, PI.GetValue(X, null), PI.GetValue(Y, null));
                            logger.Audit();
                        }
                    }
                    if (this.GlobalSettings.PropertyChain.Count > 0 && propertyList[(propertyList.Length - 1)] == PI) this.GlobalSettings.PropertyChain.RemoveAt((this.GlobalSettings.PropertyChain.Count - 1));
                }
            }
            catch (Exception ex)
            {
                this.Success = false;
                this.Exception = ex;
            }
        }

        private bool isOfSimpleType(object o)
        {
            if (o == null) return true;
                var type = o.GetType();
                return type.IsPrimitive
                                 || type == typeof(String)
                                 || type == typeof(Decimal)
                                 || type == typeof(DateTime)
                              ;
        }

        /// <summary>
        /// Check if a particular property is decorated with Audited attribute
        /// </summary>
        /// <param name="property">The reflected property to be checked</param>
        /// <returns>Boolean indicating if it has the Audited attribute</returns>
        private bool IsAudited(PropertyInfo property)
        {
            bool result = false;
            foreach (Attribute att in property.GetCustomAttributes(typeof(Audited), true))
            {
                if (!result && att is Audited) result = true;
            }
            return result;
        }
        /// <summary>
        /// Check if a particular field is decorated with Audited attribute
        /// </summary>
        /// <param name="field">The reflected field to be checked</param>
        /// <returns>Boolean indicating if it has the Audited attribute</returns>
        private bool IsAudited(FieldInfo field)
        {
            bool result = false;
            foreach (Attribute att in field.GetCustomAttributes(typeof(Audited), true))
            {
                if (!result && att is Audited) result = true;
            }
            return result;
        }

        private void Compare(TypePropertyInfoPair typePI)
        {
            IComparable valx = typePI.PropertyInfo.GetValue(X, null) as IComparable;
            var valy = typePI.PropertyInfo.GetValue(Y, null);
            ChangeLog log;
            string propertyChain = string.Empty;
            foreach (PropertyInfo PI in this.GlobalSettings.PropertyChain)
            {
                propertyChain = string.Concat(propertyChain, PI.Name, ":");
            }
            
            if (valx == null && valy == null) return;
            if (valx == null && valy != null)
            {
                log = new ChangeLog(GlobalSettings.MainObject.GetType().Name, Id, ((GlobalSettings.MainObject.GetType()==X.GetType()) ? string.Empty : propertyChain) + typePI.PropertyInfo.Name, string.Empty, valy.ToString());
                GlobalSettings.Changes.Add(log);
            }
            else if (valx != null && valy == null)
            {
                log = new ChangeLog(GlobalSettings.MainObject.GetType().Name, Id, ((GlobalSettings.MainObject.GetType() == X.GetType()) ? string.Empty : propertyChain) + typePI.PropertyInfo.Name, valx.ToString(), string.Empty);
                GlobalSettings.Changes.Add(log);
            }
            else
            {
                if (valx.CompareTo(valy) != 0)
                {
                    log = new ChangeLog(GlobalSettings.MainObject.GetType().Name, Id, ((GlobalSettings.MainObject.GetType() == X.GetType()) ? string.Empty : propertyChain) + typePI.PropertyInfo.Name, valx.ToString(), valy.ToString());
                    GlobalSettings.Changes.Add(log);
                }
            }
        }

    }

    /// <summary>
    /// Class to hold a Type and Object Info pair
    /// </summary>
    public class TypePropertyInfoPair
    {
        internal Type Type { get; set; }
        internal PropertyInfo PropertyInfo { get; set; }
        private FieldInfo FieldInfo { get; set; }

        /// <summary>
        /// Create a new instanceof TypePropertyInfo for FieldInfo
        /// </summary>
        /// <param name="type">The object type</param>
        /// <param name="fieldInfo">The FieldInfo</param>
        public TypePropertyInfoPair(Type type, FieldInfo fieldInfo)
        {
            this.Type = type;
            this.FieldInfo = fieldInfo;
        }

        /// <summary>
        /// Create a new instanceof TypePropertyInfo for PropertyInfo
        /// </summary>
        /// <param name="type">The object type</param>
        /// <param name="propertyInfo">The PropertyInfo</param>
        public TypePropertyInfoPair(Type type, PropertyInfo propertyInfo)
        {
            this.Type = type;
            this.PropertyInfo = propertyInfo;
        }
    }
}
