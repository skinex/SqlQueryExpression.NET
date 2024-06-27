namespace Ap.Tools.TSqlService.Abstractions.Models
{
    /// <summary>
    /// An representation of primary key column
    /// it holds name of column and it's typed value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PrimaryAttribute<T>
    {
        public string AttributeName { get; }
        public T AttributeValue { get; }

        public PrimaryAttribute(string attributeName, T attributeValue)
        {
            AttributeName = attributeName;
            AttributeValue = attributeValue;
        }
    }
}