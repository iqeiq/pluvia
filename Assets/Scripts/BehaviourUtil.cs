using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BehaviourUtil : MonoBehaviour {
  
    protected float posX {
        set {
            var p = transform.position;
            p.x = value;
            transform.position = p;
        }
        get { return transform.position.x; }
    }

    protected float posY {
        set {
            var p = transform.position;
            p.y = value;
            transform.position = p;
        }
        get { return transform.position.y; }
    }

    protected float posZ {
        set {
            var p = transform.position;
            p.z = value;
            transform.position = p;
        }
        get { return transform.position.z; }
    }

    protected void SetColA(float a) {
        var sr = GetComponent<SpriteRenderer>();
        var c = sr.color;
        c.a = a;
        sr.color = c;
    }

    public static bool IsTag(GameObject g, params string[] tags) {
        return tags.Any(t => g.tag.Equals(t));
    } 

    public static bool IsLayer(GameObject g, params string[] layers) {
        return layers
            .Select(l => LayerMask.NameToLayer(l))
            .Any(l => g.layer.Equals(l));
    } 

    public static int genLayerMask(int init, params string[] layers) {
        return layers
            .Select(l => LayerMask.NameToLayer(l))
            .Aggregate(init, (a, b)=> a + b);
    }

    public T FindComponent<T>(string name)
        where T : MonoBehaviour
    {
        return transform.Find(name).GetComponent<T>();
    }

    public static Vector3 SetX(Vector3 vec, float val) {
        vec.x = val;
        return vec;
    }

    public static float EaseInOut(float t) {
        return (t * t) * (3f - (2f * t));
    }

}
