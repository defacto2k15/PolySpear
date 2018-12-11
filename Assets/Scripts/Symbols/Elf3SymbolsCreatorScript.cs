using UnityEngine;

namespace Assets.Scripts.Symbols
{
    public class Elf3SymbolsCreatorScript : MonoBehaviour // this should be set in editor, not in script
    {
        public GameObject SpearSymbolPrefab; 
        public GameObject SwordSymbolPrefab; 
        public GameObject ShieldSymbolPrefab; 
        public GameObject PushSymbolPrefab; 
        public void Start()
        {
            AddSymbol(Instantiate(ShieldSymbolPrefab, transform), Orientation.NE, new ShieldEffectContainer());
            AddSymbol(Instantiate(SpearSymbolPrefab, transform), Orientation.N, new SpearEffectContainer());
            AddSymbol(Instantiate(ShieldSymbolPrefab, transform), Orientation.NW, new ShieldEffectContainer());
        }

        private void AddSymbol(GameObject prefab, Orientation orientation,  IEffectContainer effectContainer)
        {
            var newSymbol = prefab;
            newSymbol.GetComponent<SymbolModel>().Orientation = orientation;
            newSymbol.GetComponent<SymbolModel>().EffectContainer = effectContainer;
        }
    }
}