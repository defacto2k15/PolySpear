using UnityEngine;

namespace Assets.Scripts.Symbols
{
    public class Elf1SymbolsCreatorScript : MonoBehaviour // this should be set in editor, not in script
    {
        public GameObject SpearSymbolPrefab; 
        public GameObject SwordSymbolPrefab; 
        public GameObject ShieldSymbolPrefab; 
        public GameObject PushSymbolPrefab; 
        public void Start()
        {
            AddSymbol(Instantiate(SpearSymbolPrefab, transform), Orientation.N, new SpearEffectContainer());
        }

        private void AddSymbol(GameObject prefab, Orientation orientation, SpearEffectContainer effectContainer)
        {
            var newSymbol = prefab;
            newSymbol.GetComponent<SymbolModel>().Orientation = orientation;
            newSymbol.GetComponent<SymbolModel>().EffectContainer = effectContainer;
        }
    }
}