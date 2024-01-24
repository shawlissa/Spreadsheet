// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)
// Version 1.2 - Daniel Kopta
// (Clarified meaning of dependent and dependee.)
// (Clarified names in solution/project structure.)
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
namespace SpreadsheetUtilities
{
    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// t1 depends on s1; s1 must be evaluated before t1
    ///
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings. Two ordered pairs
/// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
/// Recall that sets never contain duplicates. If an attempt is made to add an element to a
/// set, and the element is already in the set, the set remains unchanged.
///
/// Given a DependencyGraph DG:
///
/// (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
/// (The set of things that depend on s)
///
/// (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
/// (The set of things that s depends on)
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    // dependents("a") = {"b", "c"}
    // dependents("b") = {"d"}
    // dependents("c") = {}
    // dependents("d") = {"d"}
    // dependees("a") = {}
    // dependees("b") = {"a"}
    // dependees("c") = {"a"}
    // dependees("d") = {"b", "d"}
    /// </summary>
public class DependencyGraph
    {
        List<KeyValuePair<string, string>> graphPairs;
        Dictionary<string, List<string>> dependents;
        Dictionary<string, List<string>> dependees;
        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            graphPairs = new List<KeyValuePair<string, string>>();
            dependents = new Dictionary<string, List<string>>();
            dependees = new Dictionary<string, List<string>>();
        }
        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get { return graphPairs.Count; }
        }
        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer. If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s]
        { 
            get { return dependees[s].Count; }
        }
        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            if (!dependents.ContainsKey(s))
                return false;
            List<string> dpnts = dependents[s];
            return dpnts.Count > 0;
        }
        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {
            if (!dependees.ContainsKey(s))
                return false;
            List<string> dpnde = dependees[s];
            return dpnde.Count > 0;
        }
        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            if (!dependents.ContainsKey(s))
                return Enumerable.Empty<string>();
            IEnumerable<string> ie = dependents[s];
            return ie.ToList<string>();
        }
        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            if (!dependees.ContainsKey(s)) 
                return Enumerable.Empty<string>();
            IEnumerable<string> ie = dependees[s];
            return ie.ToList<string>();
        }
        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        ///
        /// <para>This should be thought of as:</para>
        ///
        /// t depends on s
        ///
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S</param>
        /// <param name="t"> t cannot be evaluated until s is</param> ///
        public void AddDependency(string s, string t)
        {
            KeyValuePair<string, string> kvp = new KeyValuePair<string, string>(s, t);
            //Pair exists already, do not add doubles and instead return
            if (graphPairs.Contains(kvp))
                return;
            graphPairs.Add(kvp);
            //Dependents already has 's' as a key -> add 't' to value list.
            if (dependents.ContainsKey(s))
            {
                dependents[s].Add(t);
            } else 
            { 
                //Adds dependancy s:t to dependant list
                List<string> currSDependees = new List<string>();
                currSDependees.Add(t);
                dependents.Add(s, currSDependees);
            }

            if (dependees.ContainsKey(t))
            {
                dependees[t].Add(s);
            } else
            {
                //Adds dependee t:s to dependee list
                List<string> currSDependents = new List<string>();
                currSDependents.Add(s);
                dependees.Add(t, currSDependents);
            }
        }
        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            //If dependent 's' exists -> find t & remove; else return.
            if (dependents.ContainsKey(s))
            {
                List<string> currSDependents = dependents[s];
                foreach(string d in currSDependents) 
                {
                    if (d.Equals(t))
                    {
                        dependents[s].Remove(t);
                        break;
                    }
                }
            } else
            {
                return;
            }
            //If dependee 't' exists -> find s & remove
            if (dependees.ContainsKey(t))
            {
                List<string> currSDependees = dependees[t];
                foreach (string d in currSDependees)
                {
                    if (d.Equals(s))
                    {
                        dependees[t].Remove(s);
                        break;
                    }
                }
            }
            //Removes (s,t) as a pair 
            KeyValuePair<string, string> kvp = new KeyValuePair<string, string>(s, t);
            graphPairs.Remove(kvp);
        }
        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r). Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {

            if (!dependents.ContainsKey(s))
                dependents.Add(s, new List<string>());
            //Removes all dependents from 's' dependent list.
            dependents[s].Clear();

            //Removes all pairs beginning with 's' as key.
            HashSet<KeyValuePair<string, string>> temp = graphPairs.ToHashSet<KeyValuePair<string, string>>();
            foreach (KeyValuePair<string, string> kvp in temp)
            {
                if (kvp.Key.Equals(s))
                {
                    graphPairs.Remove(kvp);
                }
            }

            //Add all new dependents into dependents[s] and graphPairs.
            foreach (string d in newDependents)
            {
                dependents[s].Add(d);
                if (!dependees.ContainsKey(d))
                    dependees.Add(d, new List<string>());
                dependees[d].Add(s);
                KeyValuePair<string, string> kvp = new KeyValuePair<string, string>(s, d);
                graphPairs.Add(kvp);
            }
        }
        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s). Then, for each
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            if (!dependees.ContainsKey(s))
                dependees.Add(s, new List<string>());
            //Removes all dependents from 's' dependent list.
            dependees[s].Clear();

            //Removes all pairs beginning with 's' as key.
            HashSet<KeyValuePair<string,string>> temp = graphPairs.ToHashSet<KeyValuePair<string,string>>();
            foreach (KeyValuePair<string,string> kvp in temp)
            {
                if (kvp.Key.Equals(s))
                {
                    graphPairs.Remove(kvp);
                }
            }

            //Add all new dependents into dependents[s] and graphPairs.
            foreach (string d in newDependees)
            {
                dependees[s].Add(d);
                if (!dependents.ContainsKey(d))
                    dependents.Add(d, new List<string>());
                dependents[d].Add(s);
                KeyValuePair<string, string> kvp = new KeyValuePair<string, string>(d,s);
                graphPairs.Add(kvp);
            }
        }
    }
}