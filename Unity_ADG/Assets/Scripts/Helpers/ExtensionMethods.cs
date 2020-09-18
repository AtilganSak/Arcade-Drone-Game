using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if TextMeshPro
#endif

public enum BlendMode { Alpha = 0, Premultiply = 1, Additive = 2, Multiply = 3 }
public enum SurfaceType { Alpha = 0, Transparent = 1 }
public enum ValidateType { NullReference, ArgNullReference, Exception }
public enum Operator { Less, Greater, Equality, NotEqual }
public static class ExtensionMethods
{
    private static System.Random mRandom = new System.Random();
    private static string URP_STD_SHADER_NAME = "Universal Render Pipeline/Lit";

    #region Gameobject
    public static void FixNameForClone(this GameObject obj)
    {
        string newName = obj.name.Remove(obj.name.Length - 7);
        obj.name = newName;
    }
    public static bool HasComponent<T>(this GameObject gameObject, T component = default)
    {
        T temp = gameObject.GetComponent<T>();
        if(temp == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public static Vector3 DirectEuler(this Transform t)
    {
        return new Vector3(((t.localEulerAngles.x > 180) ? t.localEulerAngles.x - 360 : t.localEulerAngles.x),
                ((t.localEulerAngles.y > 180) ? t.localEulerAngles.y - 360 : t.localEulerAngles.y),
                ((t.localEulerAngles.z > 180) ? t.localEulerAngles.z - 360 : t.localEulerAngles.z));
    }
    #endregion
    #region Material
    public static void SetColor(this Material material, Color color)
    {
        if(material.shader.name == URP_STD_SHADER_NAME)
        {
            material.SetColor("_BaseColor", color);
        }
    }
    public static void SetColor(this Material material, float r, float g, float b, float a)
    {
        if(material.shader.name == URP_STD_SHADER_NAME)
        {
            material.SetColor("_BaseColor", new Color(r, g, b, a));
        }
    }
    public static Color GetColor(this Material material)
    {
        if(material.shader.name == URP_STD_SHADER_NAME)
        {
            return material.GetColor("_BaseColor");
        }
        return new Color(1, 1, 1, 1);
    }
    public static void SetSurfaceType(this Material material, SurfaceType surfaceType)
    {
        if(material.shader.name == URP_STD_SHADER_NAME)
        {
            material.SetFloat("_Surface", (int)surfaceType);
        }
        else
        {
            throw new Exception("Unspported Material!");
        }
    }
    public static SurfaceType GetSurfaceType(this Material material)
    {
        if(material.shader.name == URP_STD_SHADER_NAME)
        {
            return (SurfaceType)(int)material.GetFloat("_Surface");
        }
        else
        {
            throw new Exception("Unspported Material!");
        }
    }
    public static void SetBlendingMode(this Material material, BlendMode blendMode)
    {
        if(material.shader.name == URP_STD_SHADER_NAME)
        {
            if(material.GetFloat("_Surface") == (int)SurfaceType.Transparent)
            {
                material.SetFloat("_Blend", (int)blendMode);
            }
        }
        else
        {
            throw new Exception("Unspported Material!");
        }
    }
    public static BlendMode GetBlendingMode(this Material material)
    {
        if(material.shader.name == URP_STD_SHADER_NAME)
        {
            if(material.GetFloat("_Surface") == (int)SurfaceType.Transparent)
            {
                return (BlendMode)(int)material.GetFloat("_Blend");
            }
        }
        else
        {
            throw new Exception("Unspported Material!");
        }
        return BlendMode.Alpha;
    }
    public static void SetTextureOffset(this Material material, Vector2 value)
    {
        if(material.shader.name == URP_STD_SHADER_NAME)
        {
            material.SetTextureOffset("_BaseMap", value);
        }
        else
        {
            throw new Exception("Unspported Material!");
        }
    }
    public static Vector2 GetTextureOffset(this Material material)
    {
        if(material.shader.name == URP_STD_SHADER_NAME)
        {
            return material.GetTextureOffset("_BaseMap");
        }
        else
        {
            throw new Exception("Unspported Material!");
        }
    }
    #endregion
    #region Button
    public static Image SourceImage(this Button button)
    {
        return button.GetComponent<Image>();
    }
    public static void ChangeName(this Button button, string name)
    {
        if(button.transform.childCount > 0)
        {
        if(button.transform.GetChild(0).GetComponent<TMP_Text>())
        {
            button.transform.GetChild(0).GetComponent<TMP_Text>().text = name;
        }
//#if TextMeshPro
//#else
//            if(button.transform.GetChild(0).GetComponent<Text>())
//            {
//                button.transform.GetChild(0).GetComponent<Text>().text = name;
//            }
//#endif
        }
    }
    public static void ChangeImage(this Button button, Sprite sprite)
    {
        if(button.GetComponent<Image>())
        {
            if(sprite != null)
                button.GetComponent<Image>().sprite = sprite;
        }
    }
    public static void ChangeTextColor(this Button button, Color color)
    {
        if(button.transform.childCount > 0)
        {
        if(button.transform.GetChild(0).GetComponent<TMP_Text>())
        {
            button.transform.GetChild(0).GetComponent<TMP_Text>().color = color;
        }
//#if TextMeshPro
//#else
//            if(button.transform.GetChild(0).GetComponent<Text>())
//            {
//                button.transform.GetChild(0).GetComponent<Text>().color = color;
//            }
//#endif
        }
    }
    #endregion
    #region IEnumerable
    public static bool AddIfNo<TSource>(this List<TSource> list, TSource obj)
    {
        if (!list.Contains(obj))
        {
            list.Add(obj);
            return true;
        }
        return false;
    }
    public static Dictionary<TKey, TValue> ReverseDict<TKey, TValue>(this Dictionary<TKey, TValue> _source)
    {
        Dictionary<TKey, TValue> reversedDict = new Dictionary<TKey, TValue>();
        int sourceItemCount = _source.Count;
        for(int i = sourceItemCount - 1; i >= 0; i--)
        {
            KeyValuePair<TKey, TValue> kvp = _source.ElementAt(i);
            reversedDict.Add(kvp.Key, kvp.Value);
        }
        return reversedDict;
    }
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while(n > 1)
        {
            n--;
            int k = mRandom.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    public static bool DoesItHave<TSource>(this IEnumerable<TSource> source, System.Func<TSource, bool> func)
    {
        source.TryGetFirst(func, out bool found);
        return found;
    }
    public static TSource DoesItHave<TSource>(this IEnumerable<TSource> source, System.Func<TSource, bool> func, bool ovveride)
    {
        TSource first = source.TryGetFirst(func, out bool found);
        return first;
    }
    private static TSource TryGetFirst<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, out bool found)
    {
        if (source == null)
        {
            throw new ArgumentNullException();
        }

        if (predicate == null)
        {
            throw new ArgumentNullException();
        }

        foreach (TSource element in source)
        {
            if (predicate(element))
            {
                found = true;
                return element;
            }
        }

        found = false;
        return default;
    }
    #endregion
    #region Vector3
    public static void AddValueToVector(this Vector3 vector, float value)
    {
        vector.x += value;
        vector.y += value;
        vector.z += value;
    }
    public static Vector3 AddValueToVector(this Vector3 vector, int value)
    {
        return new Vector3(vector.x + value, vector.y + value, vector.z + value);
    }
    public static Vector2 AddValueToVector(this Vector2 vector, float value)
    {
        return new Vector2(vector.x + value, vector.y + value);
    }
    public static Vector2 AddValueToVector(this Vector2 vector, int value)
    {
        return new Vector2(vector.x + value, vector.y + value);
    }

    public static Vector3 AddValueX(this Vector3 vector, float value)
    {
        return new Vector3(vector.x + value, vector.y, vector.z);
    }
    public static Vector3 AddValueY(this Vector3 vector, float value)
    {
        return new Vector3(vector.x, vector.y + value, vector.z);
    }
    public static Vector3 AddValueZ(this Vector3 vector, float value)
    {
        return new Vector3(vector.x, vector.y, vector.z + value);
    }
    public static bool CompareVector(this Vector3 a, Vector3 b, Operator oprt)
    {
        bool result = false;
        if(a != Vector3.zero && b != Vector3.zero)
        {
            switch(oprt)
            {
                case Operator.Less:
                    if(a.x < b.x && a.y < b.y && a.z < b.z) result = true;
                    break;
                case Operator.Greater:
                    if(a.x > b.x && a.y > b.y && a.z > b.z) result = true;
                    break;
                case Operator.Equality:
                    if(a.x == b.x && a.y == b.y && a.z == b.z) result = true;
                    break;
                case Operator.NotEqual:
                    if(a.x != b.x && a.y != b.y && a.z != b.z) result = true;
                    break;
                default:
                    break;
            }
        }
        return result;
    }
    public static void ABS(this Vector3 v)
    {
        v.x = Math.Abs(v.x);
        v.y = Math.Abs(v.y);
        v.z = Math.Abs(v.z);
    }
    public static void InverseABS(this Vector3 v)
    {
        v.x = -Math.Abs(v.x);
        v.y = -Math.Abs(v.y);
        v.z = -Math.Abs(v.z);
    }
    #endregion
}