using System;
using System.Data;
using System.Collections;
namespace EBSCore.AdoClass
{
    public class TableField
    {
        private string FieldName;
        private SqlDbType FieldType;
        private object FieldValue;
        private Byte[] FieldValues;
        private bool FieldSaveEmptyValues;
        public TableField(string Name, SqlDbType Type, bool SaveEmptyValues = false)
        {
            FieldName = Name;
            FieldType = Type;
            FieldSaveEmptyValues = SaveEmptyValues;
            FieldValue = "";
        }
        public string Name
        {
            get
            {
                return FieldName;
            }
        }

        public SqlDbType Type
        {
            get
            {
                return FieldType;
            }
        }

        public object Value
        {
            get
            {
                return FieldValue;
            }
        }
        public void SetValue(string Value, ref ArrayList FieldsArrayList)
        {
            if ((FieldSaveEmptyValues | Value != "") & Value != "-1")
            {
                FieldValue = Value;
                FieldsArrayList.Add(this);
            }
        }

        public void SetBytes(Byte[] Value, ref ArrayList FieldsArrayList)
        {
            if ((FieldSaveEmptyValues | Value != null))
            {
                FieldValue = Value;
                FieldsArrayList.Add(this);
            }
        }
    }
}