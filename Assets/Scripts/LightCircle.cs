using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace kbradu
{
    public class LightCircle : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer lightOn;
        [SerializeField] private Color red = Color.red;
        [SerializeField] private Color green = Color.green;
        [SerializeField] private Color yellow = Color.yellow;
        [SerializeField] private Color blue = Color.blue;

        public void TurnOff()
        {
            lightOn.color = new Color(0, 0, 0, 0);
        }
        public void TurnRed()
        {
            lightOn.color = red;
        }
        public void TurnGreen() =>
            lightOn.color = green;
        public void TurnYellow() => lightOn.color = yellow;
          
        public void TurnBlue() => lightOn.color = blue;
    }

}
