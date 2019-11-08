using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QAObject : MonoBehaviour {

	public Text m_questionText;
	public Dropdown m_dropDownAnswer;
    public InputField userInputField;
    public PopUpQuestions questionAnswerObj;

	private void Awake(){
	}
	
	private void OnDisable()
	{
        m_questionText.text = "";
        m_dropDownAnswer.ClearOptions();
        userInputField.text = "";
	}

	void OnDestroy(){
		questionAnswerObj = null;
	}

    public void FillData(PopUpQuestions qAobj)
    {

        questionAnswerObj = qAobj;

        m_questionText.text = qAobj.QuestionText;

        if (qAobj.AnswerText != null && qAobj.AnswerText.Length > 0){

            m_dropDownAnswer.gameObject.SetActive(true);
            userInputField.gameObject.SetActive(false);

            foreach (string ans in questionAnswerObj.AnswerText)
            {
                m_dropDownAnswer.options.Add(new Dropdown.OptionData(ans));
            }
            m_dropDownAnswer.RefreshShownValue();
        } else {

            userInputField.gameObject.SetActive(true);
            m_dropDownAnswer.gameObject.SetActive(false);
        }


	}

}
	
