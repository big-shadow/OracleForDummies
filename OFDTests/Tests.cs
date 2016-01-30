using System.Collections.Generic;
using OFD;
using OFD.Transact;

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
            const int iterations = 1000;

            // Drops the table.
            tests.Add(new Test("Drop Model Table", delegate
            {
                Transactor.Drop(typeof(Thing));

                return true;
            }));

            // Inserts Models.
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

            // Updates Models.
            tests.Add(new Test("Update Model", delegate
            {
                Thing thing = new Thing();

                for (int x = 1; x <= iterations; x++)
                {
                    thing = new Thing(x);
                    thing.Name = "Ray" + " " + x;
                    thing.Save();
                }

                return thing.ID == iterations;
            }));

            // Sets an object using an ID.
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

            // Sets an object using a constructor argument.
            tests.Add(new Test("Constructor Set Where ID", delegate
            {
                Thing thing = new Thing(1);

                for (int x = 1; x <= iterations; x++)
                {
                    thing = new Thing(x);
                }

                return thing.Name.Equals("Ray" + " " + iterations);
            }));

            // Sets an object using a developer-defined query condition.
            tests.Add(new Test("Set Where", delegate
            {
                Thing thing = new Thing();

                for (int x = 1; x <= iterations; x++)
                {
                    thing.SetWhere("Name = 'Ray" + " " + iterations + "'");
                }

                return thing.ID == iterations;
            }));

            // Tests the reading and writing of properties with very long names.
            tests.Add(new Test("Scalar Long Identifier", delegate
            {
                Thing thing = new Thing(1);
                thing.Name = "Ray";
                thing.PropertyNameThatIsLongerThanThirtyCharacters = "This";
                thing.Save();

                thing.SetWhereID(1);

                return thing.PropertyNameThatIsLongerThanThirtyCharacters.Equals("This");
            }));

            // Tests the correct values are assigned to a property returned by a static call.
            tests.Add(new Test("Static Scalar Where ID Value", delegate
            {
                Thing thing = new Thing();

                for (int x = 1; x <= iterations; x++)
                {
                    thing = Thing.ScalarWhereID<Thing>(x);
                }

                return thing.ID == iterations;
            }));

            // Tests the instance type returned by a static call.
            tests.Add(new Test("Static Scalar Where ID Type", delegate
            {
                Thing thing = Thing.ScalarWhereID<Thing>(1);

                return thing.GetType().Equals(typeof(Thing));
            }));

            // Gets an instance statically with query conditions.
            tests.Add(new Test("Static Scalar Where", delegate
            {
                Thing thing = new Thing();

                for (int x = 1; x <= iterations; x++)
                {
                    thing = Thing.ScalarWhere<Thing>("Name = 'Ray" + " " + iterations + "'");
                }

                return thing.ID == iterations;
            }));

            // Gets a collection of the static caller's type with query conditions.
            tests.Add(new Test("Static Get Collection", delegate
            {
                return Thing.GetWhere<Thing>("Name LIKE '%Ray%'").Count == iterations;
            }));

            // Deletes records according to conditions.
            tests.Add(new Test("Static Delete Where", delegate
            {
                Thing.DeleteWhere<Thing>("Name LIKE '%Ray%'");

                return Transactor.GetGeneric("count(*)", "thing")[0][0].ToString().Equals("0");
            }));
        }
    }
}
