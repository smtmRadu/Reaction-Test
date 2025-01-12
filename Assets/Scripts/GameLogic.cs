using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kbradu
{
    public class GameLogic : MonoBehaviour
    {
        [SerializeField] List<LightCircle> lightCircles;
        [SerializeField] GameObject touchingWarningLight;
       
        [SerializeField, ViewOnly] List<float> results = new List<float>();

        [SerializeField] TMPro.TMP_Text infoText;
        [SerializeField] TMPro.TMP_Text fpsText;
        [SerializeField] TMPro.TMP_Text resultsText;

        [SerializeField, ViewOnly] GameState gamestate = GameState.Off;
        [SerializeField, ViewOnly] int test_count = 0;
        [SerializeField] float sleep_time = 1f;
    // Start is called before the first frame update
        void Start()
        {
            Application.targetFrameRate = 360;

        }

        // Update is called once per frame
        void Update()
        {
            if(Input.touchCount == 0)            
                touchingWarningLight.SetActive(false);       
            else        
                touchingWarningLight.SetActive(true);
            

            if(fpsText.gameObject.activeSelf)
            {
                if (gamestate == GameState.Off)
                    fpsText.text = $"{(1f / Time.deltaTime).ToString("0.0")} FPS";
                else
                    fpsText.text = "";
            }
            

            if (gamestate == GameState.WaitsForRelease)
                WaitForReleaseState();

            if (gamestate == GameState.On)
                OnState();

            if (gamestate == GameState.Off)
                OffState();       
        }


        private float start_timer = 0f;
        float turnoff_time = 0f;
        private void OnState()
        {
            if(Input.touchCount == 0)
            {
                CancelTest();
                return;
            }

            start_timer += Time.deltaTime;

            if (start_timer >= 1f)
                lightCircles[1].TurnRed();
            else return;
            if (start_timer >= 2f)
                lightCircles[2].TurnRed();
            else return;
            if (start_timer >= 3f)
                lightCircles[3].TurnRed();
            else return;
            if (start_timer >= 4f)
                lightCircles[4].TurnRed();
            else return;
            if (start_timer >= turnoff_time)
            {
                gamestate = GameState.WaitsForRelease;
                foreach (var light in lightCircles)
                {
                    light.TurnOff();
                }
                start_timer = 0f;
            }     
        }
        private void OffState()
        {
            if (Input.touchCount > 0) // launch start
            {
                test_count++;
                gamestate = GameState.On;
                resultsText.gameObject.SetActive(false);
                turnoff_time = Random.Range(4.2f, 7f);
                lightCircles[0].TurnRed();
                infoText.text = "Release when the lights turn off";
            }
        }

       
        private void WaitForReleaseState()
        {
            if(Input.touchCount == 0)
            {
                ValidateTest(start_timer);
                gamestate = GameState.Sleep;
                StartCoroutine(IntermediateTime(false));
            }

            start_timer += Time.deltaTime;
        }

        private void CancelTest()
        {
            results.Add(-1f);
            RemoveOldResultsAndRecalculateAverage();
            resultsText.text += $"<b>({test_count})</b> - \n";
            gamestate = GameState.Sleep;
            start_timer = 0f;
            infoText.text = "Early Release";
            StartCoroutine(IntermediateTime(true));

            
        }
        private void ValidateTest(float elapsedTime)
        {
            results.Add(elapsedTime);
            RemoveOldResultsAndRecalculateAverage();
            resultsText.text += $"<b>({test_count})</b> {elapsedTime.ToString("0.00000")}s \n";
            gamestate = GameState.Sleep;
            start_timer = 0f;
            infoText.text = $"{elapsedTime.ToString("0.00000")}s";
            
        }

        private void RemoveOldResultsAndRecalculateAverage()
        {
            if (test_count > 5)
            {
                int i = 1;
                bool second = false;
                for (; i < resultsText.text.Length; i++)
                {
                    if (resultsText.text[i] == '\n')
                        if (!second)
                            second = true;
                        else
                            break;
                }

                resultsText.text = resultsText.text.Remove(0, i + 1);
            }

            if (test_count > 4)
                resultsText.text = resultsText.text.Insert(0, $"<b>Average {ComputeAverageResult().ToString("0.00000")}s<b>\n");

            
        }
        private float ComputeAverageResult()
        {
            int validated = 0;
            float sum = 0f;
            foreach (var item in results)
            {
                if (item != -1)
                {
                    validated++;
                    sum += item;
                }
            }

            if (validated == 0)
                return 0f;

            return sum / validated;
        }
        IEnumerator IntermediateTime(bool canceled_test)
        {
            resultsText.gameObject.SetActive(true);
            foreach (var light in lightCircles)
            {
                if (canceled_test) light.TurnYellow();
                else light.TurnBlue();
            }
                

            yield return new WaitForSeconds(sleep_time);

            foreach (var light in lightCircles)
                light.TurnOff();

            infoText.text = "Press and Hold to Start";
            gamestate = GameState.Off;
        }
    }

  

    enum GameState
    {
        On,
        Off,
        WaitsForRelease,
        Sleep, // intermediate time between tests
    }
}
