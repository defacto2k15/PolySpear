using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.ScenarioTesting
{
    public class TestingScreenView : MonoBehaviour
    {
        public void ShowTestTitle(String title)
        {
            gameObject.SetActive(true);
            GetComponentInChildren<Text>().text = title;
        }
    }
}
