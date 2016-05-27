using System;
using System.Linq;
using System.Reflection;
using System.Text;
using Composite.C1Console.Security;
using Composite.Core.Types;


namespace Composite.Data
{
    /// <summary>
    /// EntityToken that represents a C1 Data item. EntityToken is used through out Composite C1 to describe artifacts that can have security settings and be navigated and this class make it easy
    /// to move between data items and EntityToken.
    /// </summary>
    [SecurityAncestorProvider(typeof(DataSecurityAncestorProvider))]
    public sealed class DataEntityToken : EntityToken
    {
        private IData _data;
        private bool _dataInitialized;
        private string _serializedDataSourceId;
        private string _serializedId = null; 
        private string _serializedVersionId = null;
        private string _serializedInterfaceType = null;
        private Type _interfaceType = null;
        private DataSourceId _dataSourceId = null;


        internal DataEntityToken(IData data)
        {
            Verify.ArgumentNotNull(data, "data");

            _data = data;
            _dataInitialized = true;            
            _serializedDataSourceId = null;
            _dataSourceId = _data.DataSourceId;
            Verify.ArgumentCondition(_dataSourceId != null, "data", "DataSourceId can not be null");

            _interfaceType = _dataSourceId.InterfaceType;
        }



        private DataEntityToken(string serializedDataSourceId)
        {
            _data = null;
            _dataInitialized = false;
            _serializedDataSourceId = serializedDataSourceId;
            _dataSourceId = null;
        }



        /// <exclude />
        public override string Type
        {
            get
            {
                if (_serializedInterfaceType == null)
                {
                    _serializedInterfaceType = TypeManager.SerializeType(this.DataSourceId.InterfaceType);
                }

                return _serializedInterfaceType;
            }
        }



        /// <exclude />
        public override string Source
        {
            get
            {
                return this.DataSourceId.ProviderName;
            }
        }



        /// <exclude />
        public override string Id => this.SerializedId;

        /// <exclude />
        public override string VersionId => this.SerializedVersionId;

        /// <exclude />
        public override bool IsValid()
        {
            return this.Data != null;
        }



        /// <exclude />
        public Type InterfaceType
        {
            get
            {
                if (_interfaceType == null)
                {
                    if (_dataSourceId != null)
                    {
                        _interfaceType = _dataSourceId.InterfaceType;
                    }
                    else
                    {
                        _interfaceType = TypeManager.TryGetType(this.Type);
                    }
                }

                return _interfaceType;
            }
        }



        /// <summary>
        /// The <see cref="Composite.Data.DataSourceId"/> for the data object. 
        /// </summary>
        public DataSourceId DataSourceId
        {
            get
            {
                if (_dataSourceId == null)
                {
                    _dataSourceId = DataSourceId.Deserialize(_serializedDataSourceId);
                }

                return _dataSourceId;
            }
        }



        /// <exclude />
        public override string Serialize()
        {
            return this.SerializedDataSourceId;
        }



        /// <exclude />
        public static EntityToken Deserialize(string serializedData)
        {
            return new DataEntityToken(serializedData);
        }



        /// <summary>
        /// Retrieve the data object. Cast this to the expected IData interface to access the data fields.
        /// </summary>
        public IData Data
        {
            get
            {
                if (_dataInitialized == false)
                {
                    try
                    {
                        DataSourceId dataSourceId = DataSourceId.Deserialize(this.SerializedDataSourceId);

                        _data = DataFacade.GetDataFromDataSourceId(dataSourceId);
                    }
                    catch (Exception)
                    {
                        // Ignore exception - invalid data source id == no data
                    }
                    finally
                    {
                        _dataInitialized = true;
                    }
                }

                return _data;
            }
        }



        /// <exclude />
        public override void OnGetPrettyHtml(EntityTokenHtmlPrettyfier prettyfier)
        {
            prettyfier.OnWriteId = (token, helper) =>
            {
                IDataId dataId = DataIdSerializer.Deserialize(this.Id,this.VersionId);

                StringBuilder sb = new StringBuilder();
                sb.Append("<b>DataId</b><br />");
                sb.Append("<b>Type:</b> " + dataId.GetType() + "<br />");
                foreach (PropertyInfo propertyInfo in dataId.GetType().GetPropertiesRecursively())
                {
                    sb.Append("<b>" + propertyInfo.Name + ":</b> " + propertyInfo.GetValue(dataId, null).ToString() + "<br />");
                }

                helper.AddFullRow(new string[] { "<b>Id</b>", sb.ToString() });
            };
        }



        private string SerializedDataSourceId
        {
            get
            {
                if (_serializedDataSourceId == null)
                {
                    _serializedDataSourceId = this.Data.DataSourceId.Serialize();
                }

                return _serializedDataSourceId;
            }
        }



        private string SerializedId
        {
            get
            {
                if (_serializedId == null)
                {
                    var keyPropertyNames = this.Data.GetType().GetCustomAttributesRecursively<KeyPropertyNameAttribute>().Select(f=>f.KeyPropertyName);

                    _serializedId = this.DataSourceId.DataId.Serialize(keyPropertyNames);
                }

                return _serializedId;
            }
        }

        private string SerializedVersionId
        {
            get
            {
                if (_serializedVersionId == null)
                {
                    var versionKeyPropertyNames = this.Data.GetType().GetCustomAttributesRecursively<VersionKeyPropertyNameAttribute>().Select(f=>f.VersionKeyPropertyName);
                    
                    _serializedVersionId = this.DataSourceId.DataId.Serialize(versionKeyPropertyNames);
                }

                return _serializedVersionId;
            }
        }
        
        private void CheckValidity()
        {
            Verify.That(IsValid(), "Failed to deserialize data from serialized data source identifier. Probably the data has been removed from data source.");
        }
    }
}
