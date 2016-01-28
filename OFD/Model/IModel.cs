namespace OFD
{
    interface IModel
    {
        int ID { get; set; }

        void Save();

        void SetWhereID(int id);

        void SetWhereCondition(string condition);

        void Drop();

    }
}
