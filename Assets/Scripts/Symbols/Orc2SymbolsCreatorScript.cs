using UnityEngine;

namespace Assets.Scripts.Symbols
{
    public class Orc2SymbolsCreatorScript : MonoBehaviour // this should be set in editor, not in script
    {
        public GameObject SpearSymbolPrefab; 
        public GameObject SwordSymbolPrefab; 
        public GameObject ShieldSymbolPrefab; 
        public GameObject PushSymbolPrefab; 
        public void Start()
        {
            AddSymbol(Instantiate(ShieldSymbolPrefab, transform), Orientation.NE, new ShieldEffectContainer());
            AddSymbol(Instantiate(PushSymbolPrefab, transform), Orientation.N, new PushEffectContainer());
            AddSymbol(Instantiate(ShieldSymbolPrefab, transform), Orientation.NW, new ShieldEffectContainer());
        }

        private void AddSymbol(GameObject prefab, Orientation orientation, IEffectContainer effectContainer)
        {
            var newSymbol = prefab;
            newSymbol.GetComponent<SymbolModel>().Orientation = orientation;
            newSymbol.GetComponent<SymbolModel>().EffectContainer = effectContainer;
        }
    }
}