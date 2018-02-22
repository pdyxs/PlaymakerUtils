using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker;
#if UNITY_EDITOR
using System.Linq;
#endif

[System.Serializable]
public abstract class FSMVariableWrapper
{
    public string name;

    protected PlayMakerFSM fsm;

    public abstract void Initialise(PlayMakerFSM fsm);

#if UNITY_EDITOR
    public abstract void AddTo(FsmVariables variables);
#endif
}

[System.Serializable]
public abstract class FSMVariableWrapper<TObjectType> : FSMVariableWrapper
{
    [SerializeField]
    protected TObjectType initialVal;

    public TObjectType val {
        get {
            return GetValue();
        }
        set {
            SetValue(value);
        }
    }

    public override void Initialise(PlayMakerFSM fsm)
    {
        this.fsm = fsm;
        if (initialVal != null)
        {
            SetValue(initialVal);
        }
    }

    protected abstract TObjectType GetValue();
    protected abstract void SetValue(TObjectType to);
}

[System.Serializable]
public class FSMIntWrapper : FSMVariableWrapper<int>
{
    protected override int GetValue()
    {
        return fsm.FsmVariables.FindFsmInt(name).Value;
    }

    protected override void SetValue(int to)
    {
        fsm.FsmVariables.FindFsmInt(name).SafeAssign(to);
    }

#if UNITY_EDITOR
    public override void AddTo(FsmVariables variables)
    {
        FsmInt v = new FsmInt(name);
        variables.IntVariables = variables.IntVariables.Concat(
            new FsmInt[] { v }).ToArray();
    }
#endif
}

[System.Serializable]
public class FSMBoolWrapper : FSMVariableWrapper<bool>
{
    protected override bool GetValue()
    {
        return fsm.FsmVariables.FindFsmBool(name).Value;
    }

    protected override void SetValue(bool to)
    {
        fsm.FsmVariables.FindFsmBool(name).SafeAssign(to);
    }

#if UNITY_EDITOR
    public override void AddTo(FsmVariables variables)
    {
        FsmBool v = new FsmBool(name);
        variables.BoolVariables = variables.BoolVariables.Concat(
            new FsmBool[] { v }).ToArray();
    }
#endif
}

[System.Serializable]
public class FSMEnumWrapper<TEnum> : FSMVariableWrapper<TEnum>
    where TEnum : System.IConvertible
{
    protected override TEnum GetValue()
    {
        return (TEnum)(object)fsm.FsmVariables.FindFsmEnum(name).Value;
    }

    protected override void SetValue(TEnum to)
    {
        fsm.FsmVariables.FindFsmEnum(name).SafeAssign(to);
    }

#if UNITY_EDITOR
    public override void AddTo(FsmVariables variables)
    {
        FsmEnum v = new FsmEnum(name);
        variables.EnumVariables = variables.EnumVariables.Concat(
            new FsmEnum[] { v }).ToArray();
    }
#endif
}

[System.Serializable]
public class FSMRectWrapper : FSMVariableWrapper<Rect>
{
    protected override Rect GetValue()
    {
        return fsm.FsmVariables.FindFsmRect(name).Value;
    }

    protected override void SetValue(Rect to)
    {
        fsm.FsmVariables.FindFsmRect(name).SafeAssign(to);
    }

#if UNITY_EDITOR
    public override void AddTo(FsmVariables variables)
    {
        FsmRect v = new FsmRect(name);
        variables.RectVariables = variables.RectVariables.Concat(
            new FsmRect[] { v }).ToArray();
    }
#endif
}

[System.Serializable]
public class FSMColorWrapper : FSMVariableWrapper<Color>
{
    protected override Color GetValue()
    {
        return fsm.FsmVariables.FindFsmColor(name).Value;
    }

    protected override void SetValue(Color to)
    {
        fsm.FsmVariables.FindFsmColor(name).SafeAssign(to);
    }

#if UNITY_EDITOR
    public override void AddTo(FsmVariables variables)
    {
        FsmColor v = new FsmColor(name);
        variables.ColorVariables = variables.ColorVariables.Concat(
            new FsmColor[] { v }).ToArray();
    }
#endif
}

[System.Serializable]
public class FSMFloatWrapper : FSMVariableWrapper<float>
{
    protected override float GetValue()
    {
        return fsm.FsmVariables.FindFsmFloat(name).Value;
    }

    protected override void SetValue(float to)
    {
        fsm.FsmVariables.FindFsmFloat(name).SafeAssign(to);
    }

#if UNITY_EDITOR
    public override void AddTo(FsmVariables variables)
    {
        FsmFloat v = new FsmFloat(name);
        variables.FloatVariables = variables.FloatVariables.Concat(
            new FsmFloat[] { v }).ToArray();
    }
#endif
}

[System.Serializable]
public class FSMStringWrapper : FSMVariableWrapper<string>
{
    protected override string GetValue()
    {
        return fsm.FsmVariables.FindFsmString(name).Value;
    }

    protected override void SetValue(string to)
    {
        fsm.FsmVariables.FindFsmString(name).SafeAssign(to);
    }

#if UNITY_EDITOR
    public override void AddTo(FsmVariables variables)
    {
        FsmString v = new FsmString(name);
        variables.StringVariables = variables.StringVariables.Concat(
            new FsmString[] { v }).ToArray();
    }
#endif
}

[System.Serializable]
public class FSMTextureWrapper : FSMVariableWrapper<Texture>
{
    protected override Texture GetValue()
    {
        return fsm.FsmVariables.FindFsmTexture(name).Value;
    }

    protected override void SetValue(Texture to)
    {
        fsm.FsmVariables.FindFsmTexture(name).SafeAssign(to);
    }

#if UNITY_EDITOR
    public override void AddTo(FsmVariables variables)
    {
        FsmTexture v = new FsmTexture(name);
        variables.TextureVariables = variables.TextureVariables.Concat(
            new FsmTexture[] { v }).ToArray();
    }
#endif
}

[System.Serializable]
public class FSMVector2Wrapper : FSMVariableWrapper<Vector2>
{
    protected override Vector2 GetValue()
    {
        return fsm.FsmVariables.FindFsmVector2(name).Value;
    }

    protected override void SetValue(Vector2 to)
    {
        fsm.FsmVariables.FindFsmVector2(name).SafeAssign(to);
    }

#if UNITY_EDITOR
    public override void AddTo(FsmVariables variables)
    {
        FsmVector2 v = new FsmVector2(name);
        variables.Vector2Variables = variables.Vector2Variables.Concat(
            new FsmVector2[] { v }).ToArray();
    }
#endif
}

[System.Serializable]
public class FSMVector3Wrapper : FSMVariableWrapper<Vector3>
{
    protected override Vector3 GetValue()
    {
        return fsm.FsmVariables.FindFsmVector3(name).Value;
    }

    protected override void SetValue(Vector3 to)
    {
        fsm.FsmVariables.FindFsmVector3(name).SafeAssign(to);
    }

#if UNITY_EDITOR
    public override void AddTo(FsmVariables variables)
    {
        FsmVector3 v = new FsmVector3(name);
        variables.Vector3Variables = variables.Vector3Variables.Concat(
            new FsmVector3[] { v }).ToArray();
    }
#endif
}

[System.Serializable]
public class FSMMaterialWrapper : FSMVariableWrapper<Material>
{
    protected override Material GetValue()
    {
        return fsm.FsmVariables.FindFsmMaterial(name).Value;
    }

    protected override void SetValue(Material to)
    {
        fsm.FsmVariables.FindFsmMaterial(name).SafeAssign(to);
    }

#if UNITY_EDITOR
    public override void AddTo(FsmVariables variables)
    {
        FsmMaterial v = new FsmMaterial(name);
        variables.MaterialVariables = variables.MaterialVariables.Concat(
            new FsmMaterial[] { v }).ToArray();
    }
#endif
}

[System.Serializable]
public class FSMQuaternionWrapper : FSMVariableWrapper<Quaternion>
{
    protected override Quaternion GetValue()
    {
        return fsm.FsmVariables.FindFsmQuaternion(name).Value;
    }

    protected override void SetValue(Quaternion to)
    {
        fsm.FsmVariables.FindFsmQuaternion(name).SafeAssign(to);
    }

#if UNITY_EDITOR
    public override void AddTo(FsmVariables variables)
    {
        FsmQuaternion v = new FsmQuaternion(name);
        variables.QuaternionVariables = variables.QuaternionVariables.Concat(
            new FsmQuaternion[] { v }).ToArray();
    }
#endif
}

[System.Serializable]
public class FSMGameObjectWrapper : FSMVariableWrapper<GameObject>
{
    protected override GameObject GetValue()
    {
        return (GameObject)fsm.FsmVariables.FindFsmGameObject(name).Value;
    }

    protected override void SetValue(GameObject to)
    {
        fsm.FsmVariables.FindFsmGameObject(name).SafeAssign(to);
    }

#if UNITY_EDITOR
    public override void AddTo(FsmVariables variables)
    {
        FsmGameObject v = new FsmGameObject(name);
        variables.GameObjectVariables = variables.GameObjectVariables.Concat(
            new FsmGameObject[] { v }).ToArray();
    }
#endif
}

[System.Serializable]
public class FSMBehaviourWrapper<TObjectType> : FSMVariableWrapper<TObjectType>
    where TObjectType : MonoBehaviour
{
    protected override TObjectType GetValue()
    {
        return ((GameObject)fsm.FsmVariables.FindFsmGameObject(name).Value)
            .GetComponent<TObjectType>();
    }

    protected override void SetValue(TObjectType to)
    {
        fsm.FsmVariables.FindFsmGameObject(name).SafeAssign(to.gameObject);
    }

#if UNITY_EDITOR
    public override void AddTo(FsmVariables variables)
    {
        FsmGameObject v = new FsmGameObject(name);
        variables.GameObjectVariables = variables.GameObjectVariables.Concat(
            new FsmGameObject[] { v }).ToArray();
    }
#endif
}



