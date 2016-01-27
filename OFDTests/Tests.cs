using OFD;
using OFD.Data;

namespace OFDTests
{
    public class Thing : Model
    {
        public string Name { get; set; }

        public Thing(int id = 0) : base(id)
        {

        }

        public Thing() : base()
        {

        }
    }

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

            tests.Add(new Test("Set Where ID", delegate
            {
                Thing thing = new Thing();
                thing.Name = "Not Joe";
                thing.SetWhereID(1);

                return thing.Name.Equals("Joe");
            }));

            tests.Add(new Test("Constructor Set Where ID", delegate
            {
                Thing thing = new Thing(1);

                return thing.Name.Equals("Joe");
            }));

            tests.Add(new Test("Set Where Condition", delegate
            {
                Thing thing = new Thing();
                thing.SetWhereCondition("Name = 'Joe'");

                return thing.Name.Equals("Joe");
            }));

            tests.Add(new Test("Static Fetch Where ID", delegate
            {
                Thing thing = Thing.GetWhereID<Thing>(1);

                return thing.Name.Equals("Joe");
            }));

            tests.Add(new Test("Static Fetch Where Condition", delegate
            {
                Thing thing = Thing.GetWhereCondition<Thing>("Name = 'Joe'");

                return thing.Name.Equals("Joe");
            }));

            tests.Add(new Test("Hash Long Identifier", delegate
            {
                string identifier = "This is most definitely longer than thirty characters.";
                identifier = SQLBuilder.Hash(identifier);

                return identifier.Length <= 30;
            }));
        }
    }
}
