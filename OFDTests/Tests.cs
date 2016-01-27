namespace OFDTests
{
    partial class Program
    {
        public static void MakeTests()
        {
            tests.Add(new Test("Insert Model", delegate
            {
                Thing thing = new Thing();
                thing.Drop();

                thing.Name = "Ray";
                thing.Save();

                return thing.ID > 0;
            }));

            tests.Add(new Test("Update Model", delegate
            {
                Thing thing = new Thing(1);
                thing.Name = "Joe";
                thing.Save();

                return thing.ID == 1;
            }));

            tests.Add(new Test("Fetch Where ID", delegate
            {
                Thing thing = new Thing();
                thing.Name = "Not Joe";
                thing.GetWhereID(1);

                return thing.Name.Equals("Joe");
            }));

            tests.Add(new Test("Constructor Fetch Where ID", delegate
            {
                Thing thing = new Thing();
                thing.GetWhereCondition("Name = 'Joe'");

                return thing.Name.Equals("Joe");
            }));
        }
    }
}
