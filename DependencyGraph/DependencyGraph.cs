﻿// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
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
        private int pairCount;
        Dictionary<string, HashSet<string>> dependents;
        Dictionary<string, HashSet<string>> dependees;

        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            pairCount = 0;
            dependents = new Dictionary<string, HashSet<string>>();
            dependees = new Dictionary<string, HashSet<string>>();
        }
        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get { return pairCount; }
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
            get { if (!dependees.ContainsKey(s))
                    return 0;
                  return dependees[s].Count; }
        }
        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            TestForNull(s);
            if (!dependents.ContainsKey(s))
                return false;
            return dependents[s].Count > 0;
        }
        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {
            TestForNull(s);
            if (!dependees.ContainsKey(s))
                return false;
            return dependees[s].Count > 0;
        }
        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            TestForNull(s);
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
            TestForNull(s);
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
            TestForNull(s);
            TestForNull(t);
            //Ensure s exists in dependents dictionary
            if (!dependents.ContainsKey(s))
                dependents.Add(s, new HashSet<string> { t });
            else
                if (dependents[s].Contains(t))
                    return;

            dependents[s].Add(t);

            //Ensures t exists in dependees dictionary
            if (!dependees.ContainsKey(t))
                dependees.Add(t, new HashSet<string> {s});
            else
                dependees[t].Add(s);

            pairCount++;

        }
        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            TestForNull(s);
            TestForNull(t);
            //Check for 's' in dependents dictionary
            if (!dependents.ContainsKey(s))
                return;
            else
                dependents[s].Remove(t);

            //Removing 's' if empty dent in dictionary
            if (!HasDependents(s))
                dependents.Remove(s);

            //Check for 's' in dependents dictionary
            if (!dependees.ContainsKey(t))
                return;
            else
                dependees[t].Remove(s);
            pairCount--;
        }
        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r). Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            TestForNull(s);
            if (!HasDependents(s))
                dependents.Add(s, new HashSet<string>());
            //Removes all dependents from 's' dependent list.
            foreach (string dent in GetDependents(s))
            {
                RemoveDependency(s, dent);
            }
            //Adds all newDependees to 's'.
            foreach (string newDent in newDependents)
            {
                AddDependency(s,newDent);
            }
        }
        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s). Then, for each
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            TestForNull(s);
            if (!HasDependees(s))
                dependees.Add(s, new HashSet<string>());
            //Removes all dependents from 's' dependent list.
            foreach(string dee in GetDependees(s))
            {
                RemoveDependency(dee, s);
            }
            //Adds all newDependees to 's'.
            foreach(string newDee in newDependees)
            {
                AddDependency(newDee, s);
            }
        }

        /// <summary>
        /// Ensures given string 's' is not null.
        /// </summary>
        /// <param name="s"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void TestForNull(string s)
        {
            if (s == null)
                throw new ArgumentNullException("Null is an invalid string value.");
        }
    }
}