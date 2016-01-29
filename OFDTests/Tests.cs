using OFD;
using OFD.Data;

namespace OFDTests
{
    public class Thing : Model
    {
        public string Name { get; set; }

        public string PropertyNameThatIsLongerThanThirtyCharacters { get; set; }

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
            const int iterations = 200;

            tests.Add(new Test("Drop Model Table", delegate
            {
                Thing thing = new Thing();
                thing.Drop();

                return true;
            }));

            tests.Add(new Test("Insert Model", delegate
            {
                Thing thing = new Thing();

                for (int x = 1; x <= iterations; x++)
                {
                    thing = new Thing();
                    thing.Name = "Not Ray" + " " + x;
                    thing.Save();
                }

                return thing.ID == iterations;
            }));

            tests.Add(new Test("Update Model", delegate
            {
                Thing thing = new Thing(1);

                for (int x = 1; x <= iterations; x++)
                {
                    thing = new Thing(x);
                    thing.Name = "Ray" + " " + x;
                    thing.Save();
                }

                return thing.ID == iterations;
            }));

            tests.Add(new Test("Set Where ID", delegate
            {
                Thing thing = new Thing();

                for (int x = 1; x <= iterations; x++)
                {
                    thing = new Thing();
                    thing.Name = "Not Joe";
                    thing.SetWhereID(x);
                }

                return thing.Name.Equals("Ray" + " " + iterations);
            }));

            tests.Add(new Test("Constructor Set Where ID", delegate
            {
                Thing thing = new Thing(1);

                for (int x = 1; x <= iterations; x++)
                {
                    thing = new Thing(x);
                }

                return thing.Name.Equals("Ray" + " " + iterations);
            }));

            tests.Add(new Test("Set Where Condition", delegate
            {
                Thing thing = new Thing();

                for (int x = 1; x <= iterations; x++)
                {
                    thing.SetWhereCondition("Name = 'Ray" + " " + iterations + "'");
                }

                return thing.ID == iterations;
            }));

            tests.Add(new Test("Static Fetch Where ID Values", delegate
            {
                Thing thing = new Thing();

                for (int x = 1; x <= iterations; x++)
                {
                    thing = Thing.GetWhereID<Thing>(x);
                }

                return thing.ID == iterations;
            }));

            tests.Add(new Test("Static Fetch Where ID Type", delegate
            {
                Thing thing = Thing.GetWhereID<Thing>(1);

                return thing.GetType().Equals(typeof(Thing));
            }));

            tests.Add(new Test("Static Fetch Where Condition", delegate
            {
                Thing thing = new Thing();

                for (int x = 1; x <= iterations; x++)
                {
                    thing = Thing.GetWhereCondition<Thing>("Name = 'Ray" + " " + iterations + "'");
                }

                return thing.ID == iterations;
            }));

            tests.Add(new Test("Hash Long Identifier", delegate
            {
                string identifier = "This is most definitely longer than thirty characters.";
                identifier = Hasher.Hash(identifier);

                return identifier.Length <= 30;
            }));

            tests.Add(new Test("Fetch Long Identifier", delegate
            {
                Thing thing = new Thing(1);
                thing.Name = "Joe";
                thing.PropertyNameThatIsLongerThanThirtyCharacters = "This";
                thing.Save();

                thing.SetWhereID(1);

                return thing.PropertyNameThatIsLongerThanThirtyCharacters.Equals("This");
            }));

        }
    }
}
