using System;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Assets.TestScene
{
    public class TestSceneNewUiScript : MonoBehaviour
    {

        private string _selectedDateString1 = "N/A";
        private string _selectedDateString2 = "N/A";

        private string _selectedTimeString1 = "N/A";
        private string _selectedTimeString2 = "N/A";

        private string[] _itemList = new string[] { "item 1", "item 2", "item 3" };

        private string _selectedItemString1 = "N/A";
        private string _selectedItemString2 = "N/A";

	    private string _selectedDateTimeString1 = "N/A";
	    private string _selectedDateTimeString2 = "N/A";


        private Text _textDate;
        private Text _textDatePredefined;
        private Text _textTime;
        private Text _textTimePredefined;
        private Text _textCustom;
        private Text _textCustomPredefined;
        private Text _textDateTime;
        private Text _textDateTimePredefined;

        private String SelectedDateString1
        {
            get { return "Selected Date: " + _selectedDateString1; }
            set
            {
                _selectedDateString1 = value;
                UpdateLabels();
            }
        }

        private String SelectedDateString2
        {
            get
            {
                return "Selected Date: " + _selectedDateString2;
            }
            set
            {
                _selectedDateString2 = value;
                UpdateLabels();
            }
        }

        private String SelectedTimeString1
        {
            get
            {
                return "Selected Time: " + _selectedTimeString1;
            }
            set
            {
                _selectedTimeString1 = value;
                UpdateLabels();
            }
        }

        private String SelectedTimeString2
        {
            get
            {
                return "Selected Time: " + _selectedTimeString2;
            }
            set
            {
                _selectedTimeString2 = value;
                UpdateLabels();
            }
        }

        private String SelectedItemString1
        {
            get
            {
                return "Selected Item: " + _selectedItemString1;
            }
            set
            {
                _selectedItemString1 = value;
                UpdateLabels();
            }
        }

        private String SelectedItemString2
        {
            get
            {
                return "Selected Item: " + _selectedItemString2;
            }
            set
            {
                _selectedItemString2 = value;
                UpdateLabels();
            }
        }

        private String SelectedDateTimeString1
        {
            get
            {
                return "Selected Date Time: " + _selectedDateTimeString1;
            }
            set
            {
                _selectedDateTimeString1 = value;
                UpdateLabels();
            }
        }

        private String SelectedDateTimeString2
        {
            get
            {
                return "Selected Date Time: " + _selectedDateTimeString2;
            }
            set
            {
                _selectedDateTimeString2 = value;
                UpdateLabels();
            }
        }

        // Use this for initialization
        void Start ()
        {
            _textDate = GameObject.Find("textDate").GetComponent<Text>();
            _textDatePredefined = GameObject.Find("textDatePredefined").GetComponent<Text>();
            _textTime = GameObject.Find("textTime").GetComponent<Text>();
            _textTimePredefined = GameObject.Find("textTimePredefined").GetComponent<Text>();
            _textCustom = GameObject.Find("textCustom").GetComponent<Text>();
            _textCustomPredefined = GameObject.Find("textCustomPredefined").GetComponent<Text>();
            _textDateTime = GameObject.Find("textDateTime").GetComponent<Text>();
            _textDateTimePredefined = GameObject.Find("textDateTimePredefined").GetComponent<Text>();

#if !UNITY_IPHONE
            GameObject.Find("panelDateTime").SetActive(false);
#endif

            UpdateLabels();
        }
	
        // Update is called once per frame
        void Update () {
	
        }

        public void OnDate(Object button)
        {   
            NativePicker.Instance.ShowDatePicker(GetScreenRect(button as GameObject), (long val) => {
                SelectedDateString1 = NativePicker.ConvertToDateTime(val).ToString("yyyy-MM-dd");
            }, () => {
                SelectedDateString1 = "canceled";
            });
        }

        public void OnDatePredefined(Object button)
        {
            NativePicker.Instance.ShowDatePicker(GetScreenRect(button as GameObject), NativePicker.DateTimeForDate(2012, 12, 23), (long val) => {
                SelectedDateString2 = NativePicker.ConvertToDateTime(val).ToString("yyyy-MM-dd");
            }, () => {
                SelectedDateString2 = "canceled";
            });
        }

        public void OnTime(Object button)
        {
            NativePicker.Instance.ShowTimePicker(GetScreenRect(button as GameObject), (long val) => {
                SelectedTimeString1 = NativePicker.ConvertToDateTime(val).ToString("H:mm");
            }, () => {
                SelectedTimeString1 = "canceled";
            });
        }

        public void OnTimePredefined(Object button)
        {
            NativePicker.Instance.ShowTimePicker(GetScreenRect(button as GameObject), NativePicker.DateTimeForTime(14, 45, 0), (long val) => {
                SelectedTimeString2 = NativePicker.ConvertToDateTime(val).ToString("H:mm");
            }, () => {
                SelectedTimeString2 = "canceled";
            });
        }

        public void OnCustom(Object button)
        {
            NativePicker.Instance.ShowCustomPicker(GetScreenRect(button as GameObject), _itemList, (long val) => {
                SelectedItemString1 = _itemList[val];
            }, () => {
                SelectedItemString1 = "canceled";
            });
        }

        public void OnCustomPredefined(Object button)
        {
            NativePicker.Instance.ShowCustomPicker(GetScreenRect(button as GameObject), _itemList, 1, (long val) => {
                SelectedItemString2 = _itemList[val];
            }, () => {
                SelectedItemString2 = "canceled";
            });
        }

        public void OnDateTime(Object button)
        {
            NativePicker.Instance.ShowDateTimePicker(GetScreenRect(button as GameObject), (long val) => {
                SelectedDateTimeString1 = NativePicker.ConvertToDateTime(val).ToString("yyyy-MM-dd H:mm");
            }, () => {
                SelectedDateTimeString1 = "canceled";
            });
        }

        public void OnDateTimePredefined(Object button)
        {
            NativePicker.Instance.ShowDateTimePicker(GetScreenRect(button as GameObject), NativePicker.DateTimeForDateTime(2012, 12, 23, 13, 45, 00), (long val) => {
                SelectedDateTimeString2 = NativePicker.ConvertToDateTime(val).ToString("yyyy-MM-dd H:mm");
            }, () => {
                SelectedDateTimeString2 = "canceled";
            });
        }

        private void UpdateLabels()
        {
            _textDate.text = SelectedDateString1;
            _textDatePredefined.text = SelectedDateString2;
            _textTime.text = SelectedTimeString1;
            _textTimePredefined.text = SelectedTimeString2;
            _textCustom.text = SelectedItemString1;
            _textCustomPredefined.text = SelectedItemString2;
            _textDateTime.text = SelectedDateTimeString1;
            _textDateTimePredefined.text = SelectedDateTimeString2;
        }

        private Rect GetScreenRect(GameObject gameObject)
        {
            RectTransform transform = gameObject.GetComponent<RectTransform>();

			Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
			Rect rect = new Rect(transform.position.x, Screen.height - transform.position.y, size.x, size.y);
			rect.x -= (transform.pivot.x * size.x);
			rect.y -= ((1.0f - transform.pivot.y) * size.y);

			return rect;
        }
    }
}
