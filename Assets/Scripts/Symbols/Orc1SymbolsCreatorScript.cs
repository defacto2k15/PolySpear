using UnityEngine;

namespace Assets.Scripts.Symbols
{
    public class Orc1SymbolsCreatorScript : MonoBehaviour // this should be set in editor, not in script
    {
        public GameObject SpearSymbolPrefab; 
        public GameObject SwordSymbolPrefab; 
        public GameObject ShieldSymbolPrefab; 
        public GameObject PushSymbolPrefab; 
        public void Start()
        {
            AddSymbol(Instantiate(SwordSymbolPrefab, transform), Orientation.N, new SwordEffectContainer());
            AddSymbol(Instantiate(SwordSymbolPrefab, transform), Orientation.NE, new SwordEffectContainer());
            AddSymbol(Instantiate(SwordSymbolPrefab, transform), Orientation.NW, new SwordEffectContainer());
        }

        private void AddSymbol(GameObject prefab, Orientation orientation, IEffectContainer effectContainer)
        {
            var newSymbol = prefab;
            newSymbol.GetComponent<SymbolModel>().Orientation = orientation;
            newSymbol.GetComponent<SymbolModel>().EffectContainer = effectContainer;
        }
    }
}