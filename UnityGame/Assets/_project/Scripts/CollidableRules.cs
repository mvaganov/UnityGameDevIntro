using NonStandard;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class CollidableRules : MonoBehaviour
{
    private static CollidableRules _instance;
    public static CollidableRules Instance {
        get {
            if (_instance) return _instance;
            _instance = FindObjectOfType<CollidableRules>();
            return _instance;
        }
    }
    private void Awake() { _instance = this; }

    [System.Serializable] public class Rule {
        public string name;
        public Collidable.Kind[] collider, collidee;
        public UnityEvent_Collidable_Array onCollision = new UnityEvent_Collidable_Array();
        [System.Serializable] public class UnityEvent_Collidable_Array : UnityEvent<Collidable[]> { }
        public Rule(string name, Collidable.Kind collider, Collidable.Kind collidee, EventBind eventBind) :
            this(name, new Collidable.Kind[] { collider }, new Collidable.Kind[] { collidee }, new EventBind[] { eventBind }) { }
        public Rule(string name, Collidable.Kind collider, Collidable.Kind collidee, UnityAction<Collidable[]> action) :
            this(name, new Collidable.Kind[] { collider }, new Collidable.Kind[] { collidee }, new UnityAction<Collidable[]>[] { action }) { }
        public Rule(string name, Collidable.Kind[] collider, Collidable.Kind[] collidee, EventBind[] eventBinds) {
            this.name = name; this.collider = collider; this.collidee = collidee; Array.ForEach(eventBinds, e=>e.Bind(onCollision));
        }
        public Rule(string name, Collidable.Kind[] collider, Collidable.Kind[] collidee, UnityAction<Collidable[]>[] actions) {
            this.name = name; this.collider = collider; this.collidee = collidee;
            Array.ForEach(actions, action=>onCollision.AddListener(action));
        }
        public bool AppliesTo(Collidable a, Collidable b) { return AppliesTo(a.kinds, b.kinds); }
        public bool AppliesTo(Collidable.Kind[] a, Collidable.Kind[] b) {
            return Enumerable.SequenceEqual(a, collider) && Enumerable.SequenceEqual(b, collidee);
        }
    }
    public List<Rule> collisionRules = new List<Rule>();
    Dictionary<Collidable.Kind, Dictionary<Collidable.Kind, HashSet<Rule>>> collisionRuleLookupTables = 
        new Dictionary<Collidable.Kind, Dictionary<Collidable.Kind, HashSet<Rule>>>();
    public void Start() {
        UpdateRuleLookupDictionary();
    }
    void UpdateRuleLookupDictionary() {
        for (int i = 0; i < collisionRules.Count; i++) {
            Rule r = collisionRules[i];
            if (!collisionRuleLookupTables.TryGetValue(r.collider[0], out Dictionary<Collidable.Kind, HashSet<Rule>> subLookupTable)) {
                subLookupTable = new Dictionary<Collidable.Kind, HashSet<Rule>>();
                collisionRuleLookupTables[r.collider[0]] = subLookupTable;
            }
            if (!subLookupTable.TryGetValue(r.collidee[0], out HashSet<Rule> ruleSet)) {
                ruleSet = new HashSet<Rule>();
                subLookupTable[r.collidee[0]] = ruleSet;
            }
            ruleSet.Add(r);
        }
    }
    /// <summary>
    /// if the rule listing is changed in the editor, the dictionaries need to update to match.
    /// </summary>
    private void OnValidate() {
        collisionRuleLookupTables.Clear();
        UpdateRuleLookupDictionary();
    }
    public void Add(Rule rule) {
        collisionRules.Add(rule);
        UpdateRuleLookupDictionary();
    }
    /// <summary>
    /// if a rule for these two colliding objects exist, don't add it.
    /// </summary>
    /// <param name="rule"></param>
    /// <returns>true if added, false if not added</returns>
    public bool AddIfMissing(Rule rule) {
        List<Rule> rules = GetRules(rule.collider, rule.collidee);
        if (rules != null && rules.Count > 0) {
            return false;
        }
        Add(rule);
        return true;
    }
    /// <summary>
    /// if a rule for these two colliding objects exist *with the same name*, don't add it.
    /// </summary>
    /// <param name="rule"></param>
    /// <returns>true if added, false if not added</returns>
    public bool AddIfMissingNamed(Rule rule) {
        List<Rule> rules = GetRules(rule.collider, rule.collidee);
        if (rules != null && rules.Count > 0) {
            Rule found = rules.Find(r=>r.name == rule.name);
            if (found != null) {
                return false;
            }
        }
        Add(rule);
        return true;
    }
    public List<Rule> GetRules(Collidable a, Collidable b) { return GetRules(a.kinds, b.kinds); }
    public List<Rule> GetRules(Collidable.Kind[] a, Collidable.Kind[] b) {
        List<Rule> foundRules = null;
        if (collisionRuleLookupTables.TryGetValue(a[0], out var subDictionary)) {
            if (subDictionary.TryGetValue(b[0], out var ruleset)) {
                foreach (Rule r in ruleset) {
                    if (r.AppliesTo(a, b)) {
                        if (foundRules == null) { foundRules = new List<Rule>(); }
                        foundRules.Add(r);
                    }
                }
            }
        }
        return foundRules;
    }
    public void DebugCollision(Collidable[] collidables) {
        Debug.Log("Collision between "+ collidables[0]+" and "+ collidables[1]);
    }
    public void DestroyCollider(Collidable[] collidables) {
        Destroy(collidables[0].gameObject);
    }
    public void DestroyCollidee(Collidable[] collidables) {
        Destroy(collidables[1].gameObject);
    }
    public void DestroyBoth(Collidable[] collidables) {
        Destroy(collidables[0].gameObject);
        Destroy(collidables[1].gameObject);
    }
}
