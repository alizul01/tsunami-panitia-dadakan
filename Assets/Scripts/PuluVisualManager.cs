using System.Collections.Generic;
using UnityEngine;

public class PuluVisualManager : SingletonMonoBehaviour<PuluVisualManager>
{
    public List<Sprite> puluDefault;
    public List<Sprite> puluWithMask;
    public Sprite GetPuluDefaultRandom => puluDefault[Random.Range(0, puluDefault.Count)];
    public Sprite GetPuluWithMaskRandom => puluWithMask[Random.Range(0, puluWithMask.Count)];
}
