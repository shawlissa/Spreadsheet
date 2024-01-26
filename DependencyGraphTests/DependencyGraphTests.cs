using SpreadsheetUtilities;
using System.Security.Cryptography.X509Certificates;

///<summary>
///Class <c> DependencyGraphTests </c> extensively tests the DependencyGraph class and its methods in the Spreadsheet solution.
///
///</summary>
namespace DevelopmentTests;

[TestClass()]
public class DependencyGraphTests
{
    IEnumerable<string> ie;
    DependencyGraph empty;
    DependencyGraph small;

    /// <summary>
    /// Creates two graphs, an empty and small one for testing.
    /// </summary>
    [TestInitialize()]
    public void TestInitialize()
    {
        empty = new DependencyGraph();

        small = new DependencyGraph();
        small.AddDependency("A1", "A5");
        small.AddDependency("A2", "A4");
        small.AddDependency("A3", "A1");
        small.AddDependency("A4", "A3");

    }
    //Tests written before adding/considering given tests by assignment 2: ------------------
    /// <summary>
    /// Empty graph should contain nothing
    /// </summary>
    [TestMethod()]
    public void TestSizeEmpty()
    {
        Assert.AreEqual(0, empty.Size);
    }

    /// <summary>
    /// Small graph should contain 4 cells
    /// </summary>
    [TestMethod()]
    public void TestSizeSmall()
    {
        Assert.AreEqual(4, small.Size);
    }

    /// <summary>
    /// Empty graph should return false as A1 nor its dependents exist.
    /// </summary>
    [TestMethod()]
    public void TestHasDependentsEmpty()
    {
        Assert.IsFalse(empty.HasDependents("A1"));
    }

    /// <summary>
    /// Empty graph should return false as A1 nor its dependees exist.
    /// </summary>
    [TestMethod()]
    public void TestHasDependeesEmpty()
    {
        Assert.IsFalse(empty.HasDependees("A1"));
    }

    /// <summary>
    /// A3, A2, A1, and A4 all have dependents whereas A5 does not.
    /// </summary>
    [TestMethod()]
    public void TestHasDependentsSmall()
    {
        Assert.IsTrue(small.HasDependents("A3"));
        Assert.IsTrue(small.HasDependents("A2"));
        Assert.IsTrue(small.HasDependents("A1"));
        Assert.IsTrue(small.HasDependents("A4"));

        //Return false as A5 has never had dependents added -> never added to dependent list as key.
        Assert.IsFalse(small.HasDependents("A5"));
    }

    /// <summary>
    /// A1, A3, A4, and A5 all have dependees whereas A2 does not.
    /// </summary>
    [TestMethod()]
    public void TestHasDependeesSmall()
    {
        Assert.IsTrue(small.HasDependees("A1"));
        Assert.IsTrue(small.HasDependees("A3"));
        Assert.IsTrue(small.HasDependees("A4"));
        Assert.IsTrue(small.HasDependees("A5"));

        //Return false as A2 has never had dependees added -> never added to dependee list as key.
        Assert.IsFalse(small.HasDependees("A2"));
    }

    /// <summary>
    /// Adds multiple dependents to cell A1.
    /// Verifies each dependent is added to A1 dependency list.
    /// </summary>
    [TestMethod()]
    public void TestAddDependencySmallCopy()
    {
        //Verifies A2 and A3 are added to A1 despite A1 already added in dependency dictionary
        ie = small.GetDependents("A1");

        Assert.AreEqual(1, ie.Count());
        Assert.IsTrue(ie.Contains("A5"));

        small.AddDependency("A1", "A2");
        ie = small.GetDependents("A1");

        Assert.AreEqual(2, ie.Count());
        Assert.IsTrue(ie.Contains("A2"));


        small.AddDependency("A1", "A3");
        ie = small.GetDependents("A1");
        Assert.AreEqual(3, ie.Count());
        Assert.IsTrue(ie.Contains("A3"));

        //Verifies A1 is added to each of its dependents 'dependee' list.
        ie = small.GetDependees("A5");
        Assert.IsTrue(ie.Contains("A1"));

        ie = small.GetDependees("A2");
        Assert.IsTrue(ie.Contains("A1"));

        ie = small.GetDependees("A3");
        Assert.IsTrue(ie.Contains("A1"));
    }

    /// <summary>
    /// Adds then removes multiple dependents from A1.
    /// Verifies each dependent was added and removed from A1 dependent list.
    /// </summary>
    [TestMethod()]
    public void TestRemoveDependencySmallCopy()
    {
        small.AddDependency("A1", "A2");
        small.AddDependency("A1", "A3");
        small.AddDependency("A1", "A4");
        ie = small.GetDependents("A1");
        //Verifying A1 has all the correct dependents
        Assert.AreEqual(4, ie.Count());
        Assert.IsTrue(ie.Contains("A5"));
        Assert.IsTrue(ie.Contains("A4"));
        Assert.IsTrue(ie.Contains("A3"));
        Assert.IsTrue(ie.Contains("A2"));

        //Now testing the removal & confirming all other objects stay in A1 dependants list.
        small.RemoveDependency("A1", "A2");
        ie = small.GetDependents("A1");
        Assert.AreEqual(3, ie.Count());
        Assert.IsFalse(ie.Contains("A2"));
        Assert.IsTrue(ie.Contains("A3"));
        Assert.IsTrue(ie.Contains("A4"));
        Assert.IsTrue(ie.Contains("A5"));

        small.RemoveDependency("A1", "A3");
        ie = small.GetDependents("A1");
        Assert.AreEqual(2, ie.Count());
        Assert.IsFalse(ie.Contains("A3"));
        Assert.IsTrue(ie.Contains("A4"));
        Assert.IsTrue(ie.Contains("A5"));

        small.RemoveDependency("A1", "A4");
        ie = small.GetDependents("A1");
        Assert.AreEqual(1, ie.Count());
        Assert.IsFalse(ie.Contains("A4"));
        Assert.IsTrue(ie.Contains("A5"));

        small.RemoveDependency("A1", "A5");
        ie = small.GetDependents("A1");
        Assert.AreEqual(0, ie.Count());
        Assert.IsFalse(ie.Contains("A5"));

    }

    /// <summary>
    /// Replaces dependents in A1
    /// </summary>
    [TestMethod()]
    public void testSmallReplaceDependency()
    {
        //A1 only has dependent A5. Should remove A5 and then add replace list into A1 dependents.
        ie = small.GetDependents("A1");
        string[] replace = { "B2", "B3", "B4", "B5" };
        //Ensures replace values are not already in A1 dependency list.
        Assert.IsFalse(ie.Contains("B2"));
        Assert.IsFalse(ie.Contains("B3"));
        Assert.IsFalse(ie.Contains("B4"));
        Assert.IsFalse(ie.Contains("B5"));
        //Ensures A3 is in A1 dependency list.
        Assert.IsTrue(ie.Contains("A5"));
        small.ReplaceDependents("A1", new HashSet<string>(replace));

        //Refreshing ie as A1 dependent values should have changed
        ie = small.GetDependents("A1");

        Assert.IsFalse(ie.Contains("A5"));
        Assert.IsTrue(ie.Contains("B2"));
        Assert.IsTrue(ie.Contains("B3"));
        Assert.IsTrue(ie.Contains("B4"));
        Assert.IsTrue(ie.Contains("B5"));
    }

    /// <summary>
    /// Replaces dependees in A1
    /// </summary>
    [TestMethod()]
    public void testSmallReplaceDependee()
    {
        //A1 only has dependent A5. Should remove A5 and then add replace list into A1 dependents.
        ie = small.GetDependees("A1");
        string[] replace = { "B2", "B3", "B4", "B5" };
        //Ensures replace values are not already in A1 dependency list.
        Assert.IsFalse(ie.Contains("B2"));
        Assert.IsFalse(ie.Contains("B3"));
        Assert.IsFalse(ie.Contains("B4"));
        Assert.IsFalse(ie.Contains("B5"));
        //Ensures A3 is in A1 dependency list.
        Assert.IsTrue(ie.Contains("A3"));
        small.ReplaceDependees("A1", new HashSet<string>(replace));

        //Refreshing ie as A1 dependent values should have changed
        ie = small.GetDependees("A1");

        Assert.IsFalse(ie.Contains("A3"));
        Assert.IsTrue(ie.Contains("B2"));
        Assert.IsTrue(ie.Contains("B3"));
        Assert.IsTrue(ie.Contains("B4"));
        Assert.IsTrue(ie.Contains("B5"));
    }
    //Assignment Provided Tests: -------------------------------------

    /// <summary>
    ///Empty graph should contain nothing
    ///</summary>
    [TestMethod()]
    public void SimpleEmptyTest()
    {
        DependencyGraph t = new DependencyGraph();
        Assert.AreEqual(0, t.Size);
    }
    /// <summary>
    ///Empty graph should contain nothing
    ///</summary>
    [TestMethod()]
    public void SimpleEmptyRemoveTest()
    {
        DependencyGraph t = new DependencyGraph();
        t.AddDependency("x", "y");
        Assert.AreEqual(1, t.Size);
        t.RemoveDependency("x", "y");
        Assert.AreEqual(0, t.Size);
    }
    /// <summary>
    ///Empty graph should contain nothing
    ///</summary>
    [TestMethod()]
    public void EmptyEnumeratorTest()
    {
        DependencyGraph t = new DependencyGraph();
        t.AddDependency("x", "y");
        IEnumerator<string> e1 = t.GetDependees("y").GetEnumerator();
        Assert.IsTrue(e1.MoveNext());
        Assert.AreEqual("x", e1.Current);
        IEnumerator<string> e2 = t.GetDependents("x").GetEnumerator();
        Assert.IsTrue(e2.MoveNext());
        Assert.AreEqual("y", e2.Current);
        t.RemoveDependency("x", "y");
        Assert.IsFalse(t.GetDependees("y").GetEnumerator().MoveNext());
        Assert.IsFalse(t.GetDependents("x").GetEnumerator().MoveNext());
    }
    /// <summary>
    ///Replace on an empty DG shouldn't fail
    ///</summary>
    [TestMethod()]
    public void SimpleReplaceTest()
    {
        DependencyGraph t = new DependencyGraph();
        t.AddDependency("x", "y");
        Assert.AreEqual(t.Size, 1);
        t.RemoveDependency("x", "y");
        t.ReplaceDependents("x", new HashSet<string>());
        t.ReplaceDependees("y", new HashSet<string>());
    }
    ///<summary>
    ///It should be possibe to have more than one DG at a time.
    ///</summary>
    [TestMethod()]
    public void StaticTest()
    {
        DependencyGraph t1 = new DependencyGraph();
        DependencyGraph t2 = new DependencyGraph();
        t1.AddDependency("x", "y");
        Assert.AreEqual(1, t1.Size);
        Assert.AreEqual(0, t2.Size);
    }
    /// <summary>
    ///Non-empty graph contains something
    ///</summary>
    [TestMethod()]
    public void SizeTest()
    {
        DependencyGraph t = new DependencyGraph();
        t.AddDependency("a", "b");
        t.AddDependency("a", "c");
        t.AddDependency("c", "b");
        t.AddDependency("b", "d");
        Assert.AreEqual(4, t.Size);
    }
    /// <summary>
    ///Non-empty graph contains something
    ///</summary>
    [TestMethod()]
    public void EnumeratorTest()
    {
        DependencyGraph t = new DependencyGraph();
        t.AddDependency("a", "b");
        t.AddDependency("a", "c");
        t.AddDependency("c", "b");
        t.AddDependency("b", "d");
        IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
        Assert.IsFalse(e.MoveNext());
        e = t.GetDependees("b").GetEnumerator();
        Assert.IsTrue(e.MoveNext());
        String s1 = e.Current;
        Assert.IsTrue(e.MoveNext());
        String s2 = e.Current;
        Assert.IsFalse(e.MoveNext());
        Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));
        e = t.GetDependees("c").GetEnumerator();
        Assert.IsTrue(e.MoveNext());
        Assert.AreEqual("a", e.Current);
        Assert.IsFalse(e.MoveNext());
        e = t.GetDependees("d").GetEnumerator();
        Assert.IsTrue(e.MoveNext());
        Assert.AreEqual("b", e.Current);
        Assert.IsFalse(e.MoveNext());
    }
    /// <summary>
    ///Non-empty graph contains something
    ///</summary>
    [TestMethod()]
    public void ReplaceThenEnumerate()
    {
        DependencyGraph t = new DependencyGraph();
        t.AddDependency("x", "b");
        t.AddDependency("a", "z");
        t.ReplaceDependents("b", new HashSet<string>());
        t.AddDependency("y", "b");
        t.ReplaceDependents("a", new HashSet<string>() { "c" });
        t.AddDependency("w", "d");
        t.ReplaceDependees("b", new HashSet<string>() { "a", "c" });
        t.ReplaceDependees("d", new HashSet<string>() { "b" });
        IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
        Assert.IsFalse(e.MoveNext());
        e = t.GetDependees("b").GetEnumerator();
        Assert.IsTrue(e.MoveNext());
        String s1 = e.Current;
        Assert.IsTrue(e.MoveNext());
        String s2 = e.Current;
        Assert.IsFalse(e.MoveNext());
        Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));
        e = t.GetDependees("c").GetEnumerator();
        Assert.IsTrue(e.MoveNext());
        Assert.AreEqual("a", e.Current);
        Assert.IsFalse(e.MoveNext());
        e = t.GetDependees("d").GetEnumerator();
        Assert.IsTrue(e.MoveNext());
        Assert.AreEqual("b", e.Current);
        Assert.IsFalse(e.MoveNext());
    }
    /// <summary>
    ///Using lots of data
    ///</summary>
   [TestMethod()]
    public void StressTest()
    {
        // Dependency graph
        DependencyGraph t = new DependencyGraph();
        // A bunch of strings to use
        const int SIZE = 200;
        string[] letters = new string[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            letters[i] = ("" + (char)('a' + i));
        }
        // The correct answers
        HashSet<string>[] dents = new HashSet<string>[SIZE];
        HashSet<string>[] dees = new HashSet<string>[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            dents[i] = new HashSet<string>();
            dees[i] = new HashSet<string>();
        }
        // Add a bunch of dependencies
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 1; j < SIZE; j++)
            {
                t.AddDependency(letters[i], letters[j]);
                dents[i].Add(letters[j]); //Should add a -> ~I to dents keys && b -> ~I to each keys value list.
                dees[j].Add(letters[i]); //Should add b -> ~I to each of dees keys && a -> ~I to each value list.
            }
        }
        // Remove a bunch of dependencies
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 4; j < SIZE; j += 4)
            {
                t.RemoveDependency(letters[i], letters[j]);
                dents[i].Remove(letters[j]); //Should remove e -> ~I in each of dents keys
                dees[j].Remove(letters[i]); //Should remove all letters from e -> ~I of dees keys
            }
        }
        // Add some back
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 1; j < SIZE; j += 2)
            {
                t.AddDependency(letters[i], letters[j]); 
                dents[i].Add(letters[j]); //Should add values b -> ~I to each dents key
                dees[j].Add(letters[i]); //Should add values a -> ~I to dees keys from b ->~I
            }
        }
        // Remove some more
        for (int i = 0; i < SIZE; i += 2)
        {
            for (int j = i + 3; j < SIZE; j += 3)
            {
                t.RemoveDependency(letters[i], letters[j]);
                dents[i].Remove(letters[j]); //Should remove every three values in every other key
                dees[j].Remove(letters[i]); //Syould remove every other two values in every three keys
            }
        }
        // Make sure everything is right
        for (int i = 1; i < SIZE; i++)
        {
            Assert.IsTrue(dents[i - 1].SetEquals(new
            HashSet<string>(t.GetDependents(letters[i - 1]))));
            Assert.IsTrue(dees[i].SetEquals(new
            HashSet<string>(t.GetDependees(letters[i]))));
        }

    }
    //Tests written after adding/considering tests provided by assignment:
  
    /// <summary>
    /// Adds and removes a small amount of data (size of 5)
    /// </summary>
    [TestMethod()]
    public void tinyStressTest()
    {
        // Dependency graph
        DependencyGraph t = new DependencyGraph();
        // A bunch of strings to use
        const int SIZE = 5;
        string[] letters = new string[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            letters[i] = ("" + (char)('a' + i));
        }
        // The correct answers
        HashSet<string>[] dents = new HashSet<string>[SIZE];
        HashSet<string>[] dees = new HashSet<string>[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            dents[i] = new HashSet<string>();
            dees[i] = new HashSet<string>();
        }
        // Add a bunch of dependencies
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 1; j < SIZE; j++)
            {
                t.AddDependency(letters[i], letters[j]);
                dents[i].Add(letters[j]); //Should add a -> ~I to dents keys && b -> ~I to each keys value list.
                dees[j].Add(letters[i]); //Should add b -> ~I to each of dees keys && a -> ~I to each value list.
            }
        }
        // Remove a bunch of dependencies
        for (int i = 0; i < SIZE; i += 2)
        {
            for (int j = 1; j < SIZE; j += 2)
            {
                t.RemoveDependency(letters[i], letters[j]);
                dents[i].Remove(letters[j]); //Should get keys a, c, e
                dees[j].Remove(letters[i]); //Should remove values b & d from keys a, c, and e.
            }
        }
        // Add all back
        for (int i = 0; i < SIZE; i += 2)
        {
            for (int j = 1; j < SIZE; j += 2)
            {
                t.AddDependency(letters[i], letters[j]);
                dents[i].Add(letters[j]); //Should get a, c, e keys
                dees[j].Add(letters[i]); //Should add values b & d back into a, c, and e keys.
            }
        }
        // Remove some more
        for (int i = 1; i < SIZE; i += 2)
        {
            for (int j = 0; j < SIZE; j += 2)
            {
                t.RemoveDependency(letters[i], letters[j]);
                dents[i].Remove(letters[j]); //Should get keys b & d
                dees[j].Remove(letters[i]); //Should remove values a, c, and e from keys b & d
            }
        }
        // Make sure everything is right
        for (int i = 1; i < SIZE; i++)
        {
            Assert.IsTrue(dents[i - 1].SetEquals(new
            HashSet<string>(t.GetDependents(letters[i - 1]))));
            Assert.IsTrue(dees[i].SetEquals(new
            HashSet<string>(t.GetDependees(letters[i]))));
        }
    }

    /// <summary>
    /// Adds and removes a small amount of data (size of 50)
    /// </summary>
    [TestMethod()]
    public void mediumStressTest()
    {
        // Dependency graph
        DependencyGraph t = new DependencyGraph();
        // A bunch of strings to use
        const int SIZE = 50;
        string[] letters = new string[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            letters[i] = ("" + (char)('a' + i));
        }
        // The correct answers
        HashSet<string>[] dents = new HashSet<string>[SIZE];
        HashSet<string>[] dees = new HashSet<string>[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            dents[i] = new HashSet<string>();
            dees[i] = new HashSet<string>();
        }
        // Add a bunch of dependencies
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 1; j < SIZE; j++)
            {
                t.AddDependency(letters[i], letters[j]);
                dents[i].Add(letters[j]); //Should add a -> ~I to dents keys && b -> ~I to each keys value list.
                dees[j].Add(letters[i]); //Should add b -> ~I to each of dees keys && a -> ~I to each value list.
            }
        }
        // Remove a bunch of dependencies
        for (int i = 0; i < SIZE; i += 2)
        {
            for (int j = 1; j < SIZE; j += 2)
            {
                t.RemoveDependency(letters[i], letters[j]);
                dents[i].Remove(letters[j]); //Should get keys a, c, e
                dees[j].Remove(letters[i]); //Should remove values b & d from keys a, c, and e.
            }
        }
        // Add all back
        for (int i = 0; i < SIZE; i += 2)
        {
            for (int j = 1; j < SIZE; j += 2)
            {
                t.AddDependency(letters[i], letters[j]);
                dents[i].Add(letters[j]); //Should get a, c, e keys
                dees[j].Add(letters[i]); //Should add values b & d back into a, c, and e keys.
            }
        }
        // Remove some more
        for (int i = 1; i < SIZE; i += 2)
        {
            for (int j = 0; j < SIZE; j += 2)
            {
                t.RemoveDependency(letters[i], letters[j]);
                dents[i].Remove(letters[j]); //Should get keys b & d
                dees[j].Remove(letters[i]); //Should remove values a, c, and e from keys b & d
            }
        }
        // Make sure everything is right
        for (int i = 1; i < SIZE; i++)
        {
            Assert.IsTrue(dents[i - 1].SetEquals(new
            HashSet<string>(t.GetDependents(letters[i - 1]))));
            Assert.IsTrue(dees[i].SetEquals(new
            HashSet<string>(t.GetDependees(letters[i]))));
        }
    }

    /// <summary>
    /// Adds dependency pairs that already exist in graph, should ignore the duplicates
    /// </summary>
    [TestMethod()]
    public void addDependencyAlreadyContainsPair()
    {
        Assert.IsTrue(small.Size == 4);
        small.AddDependency("A1", "A5");
        small.AddDependency("A2", "A4");
        small.AddDependency("A3", "A1");
        small.AddDependency("A4", "A3");
        Assert.IsTrue(small.Size == 4);
    }

    /// <summary>
    /// Tries to remove a pair with a key that doesnt exist, should do nothing
    /// </summary>
    [TestMethod()]
    public void removeDependencyDeptStringDoesntExist()
    {
        Assert.IsTrue(small.Size == 4);
        small.RemoveDependency("B5", "A1");
        Assert.IsTrue(small.Size == 4);
    }

    /// <summary>
    /// Tries to remove a pair with a value that doesnt exist, should do nothing
    /// </summary>
    [TestMethod()]
    public void removeDependencyDeeStringDoesntExist()
    {
        Assert.IsTrue(small.Size == 4);
        small.RemoveDependency("A1", "B2");
        Assert.IsTrue(small.Size == 4);
    }

    ///<summary>
    ///Gets the size of the requested dependee
    ///</summary>
    [TestMethod()]
    public void dependeesSize()
    {
        Assert.AreEqual(1, small["A1"]);
        small.AddDependency("B2", "A1");
        Assert.AreEqual(2, small["A1"]);
        small.AddDependency("A1", "B2");
        Assert.AreEqual(2, small["A1"]);
    }
/*
    /// <summary>
    /// Should fail as it should throw from null values
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(ArgumentException),
    "A null value was inappropriately added in either the key or value.")]
    public void TestForNullsAddDependency()
    {
        small.AddDependency(null, null);
    }

    /// <summary>
    /// Should fail as it should throw from null values
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(ArgumentException),
    "A null value was inappropriately added in either the key or value.")]
    public void TestForNullsRemoveDependency()
    {
        small.RemoveDependency(null, null);
    }

    /// <summary>
    /// Should fail as it should throw from null values
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(ArgumentException),
    "A null value was inappropriately added in either the key or value.")]
    public void TestForNullsReplaceDependents()
    {
        small.ReplaceDependents(null, null);
    }

    /// <summary>
    /// Should fail as it should throw from null values
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(ArgumentException),
    "A null value was inappropriately added in either the key or value.")]
    public void TestForNullsReplaceDependees()
    {
        small.ReplaceDependees(null, null);
    }

    /// <summary>
    /// Should fail as it should throw from null values
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(ArgumentException),
    "A null value was inappropriately added in either the key or value.")]
    public void TestForNullsHasDependees()
    {
        small.HasDependees(null);
    }

    /// <summary>
    /// Should fail as it should throw from null values
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(ArgumentException),
    "A null value was inappropriately added in either the key or value.")]
    public void TestForNullsHasDependents()
    {
        small.HasDependents(null);
    }

    /// <summary>
    /// Should fail as it should throw from null values
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(ArgumentException),
    "A null value was inappropriately added in either the key or value.")]
    public void TestForNullsGetDependents()
    {
        small.GetDependents(null);
    }

    /// <summary>
    /// Should fail as it should throw from null values
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(ArgumentException),
    "A null value was inappropriately added in either the key or value.")]
    public void TestForNullsGetDependees()
    {
        small.GetDependees(null);
    }
*/
}