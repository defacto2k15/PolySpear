using UnityEngine;

namespace Assets.Scripts.Symbols
{
    public class Orc3SymbolsCreatorScript : MonoBehaviour // this should be set in editor, not in script
    {
        public GameObject SpearSymbolPrefab; 
        public GameObject SwordSymbolPrefab; 
        public GameObject ShieldSymbolPrefab; 
        public GameObject PushSymbolPrefab; 
        public void Start()
        {
            AddSymbol(Instantiate(SpearSymbolPrefab, transform), Orientation.NE, new SpearEffectContainer());
            AddSymbol(Instantiate(SpearSymbolPrefab, transform), Orientation.N, new SpearEffectContainer());
            AddSymbol(Instantiate(SpearSymbolPrefab, transform), Orientation.NW, new SpearEffectContainer());
        }

        private void AddSymbol(GameObject prefab, Orientation orientation, IEffectContainer effectContainer)
        {
            var newSymbol = prefab;
            newSymbol.GetComponent<SymbolModel>().Orientation = orientation;
            newSymbol.GetComponent<SymbolModel>().EffectContainer = effectContainer;
        }
    }
}