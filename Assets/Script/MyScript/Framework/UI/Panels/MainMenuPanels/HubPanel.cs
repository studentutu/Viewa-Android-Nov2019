using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OTPL.UI;
using OTPL.modal;
using SQLite4Unity3d;
using System.Linq;
using UnityEngine.UI;
using SimpleJSON;
using UnityEngine.Networking;
using System.IO;

public class HubPanel : PanelBase
{

    public GameObject CategoryPrefab;
    public Transform parentObject;
    public MyScrollRect myScrollRect;
    Button[] buttonArray;
    List<Categories> catList = new List<Categories>();
    List<Transform> catTransformList = new List<Transform>();
    bool isContentLoaded;

    protected override void Awake()
    {
        base.Awake();

        Transform rightButton = transform.Find("NavigationBarPanel/RightButton");
        UnityEngine.UI.Image buttonImage = rightButton.GetComponent<UnityEngine.UI.Image>();
        buttonImage.sprite = AppManager.Instnace.spriteAtlas.GetSprite("Recent");
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        StopAllCoroutines();

        //TODO: Jeetesh - This condition is temporarly we need to call GetCategory() each time on OnEnable.

        GetCategory();

        AppManager.Instnace.triggerHistoryMode = eTriggerHistoryMode.TriggerHistoryModeCategory;
        CanvasManager.Instnace.ShowPanelManager(ePanelManager.BottomBarManager);
        isContentLoaded = false;
    }

    protected override void OnDisable()
    {

        StopAllCoroutines();

        base.OnDisable();
        System.GC.Collect();
        Resources.UnloadUnusedAssets();
        isContentLoaded = false;
    }
    protected override void OnUIButtonClicked(UnityEngine.UI.Button a_button)
    {
        base.OnUIButtonClicked(a_button);

        switch (a_button.name)
        {
            case "RightButton": //Recent
                Debug.Log("Button selected -" + a_button.name);
                AppManager.Instnace.triggerHistoryMode = eTriggerHistoryMode.TriggerHistoryModeRecent;
                AppManager.Instnace.categoryName = "Recent";
                CanvasManager.Instnace.ReturnPanelManager(ePanelManager.MainMenuPanelManager).NavigateToPanel(ePanels.HubDetail_Panel);
                break;

            case "categoryButton":
                Debug.Log("Button selected -" + a_button.name);
                //			Application.OpenURL ("mailto:sample@sample.com?Subject=Sample%20email&Body=Hi%20Sam%20Sample%2C%0A%0AThis%20is%20a%20sample%20body%20text...");  //"tel:123456789"
                OnCategoryClicked(a_button.transform.parent);
                break;
        }
    }

    void OnCategoryClicked(Transform categoryItemObj)
    { //Hub Details

        if ((myScrollRect.velocity.y <= 0.0f && myScrollRect.velocity.y > -0.12f) && isContentLoaded)
        {
            AppManager.Instnace.categoryName = categoryItemObj.GetChild(0).name;
            AppManager.Instnace.categoryid = long.Parse(categoryItemObj.name); //146141;
            AppManager.Instnace.triggerHistoryMode = eTriggerHistoryMode.TriggerHistoryModeChannel;

            HubDetailPanel hubDetailPanel = (HubDetailPanel)CanvasManager.Instnace.ReturnPanelManager(ePanelManager.MainMenuPanelManager).ReturnPanel(ePanels.HubDetail_Panel);
            hubDetailPanel.navigationTitle.text = AppManager.Instnace.categoryName;
            hubDetailPanel.desc = categoryItemObj.GetChild(1).GetComponent<Text>().text;
            CanvasManager.Instnace.ReturnPanelManager(ePanelManager.MainMenuPanelManager).AddPanel(ePanels.HubDetail_Panel);
        }
    }

    public void GetCategory()
    {
        AppManager.Instnace.messageBoxManager.ShowPreloaderDefault();
        string url = AppManager.Instnace.baseURL + "/cloud/categories.aspx?rid=" + AppManager.Instnace.regionId;
        Debug.Log("GetQuestionResponse url:" + url);
        WebService.Instnace.Get(url, WebCallCategories);

    }
    void WebCallCategories(string response)
    {
        Debug.Log("Response :" + response);
        JSONNode test = JSONNode.Parse(response);

        if (test != null)
        {
            //Initial when there are no elements in HUB
            if (catList.Count <= 0)
            {
                for (int i = 0; i < test.Childs.Count(); i++)
                {
                    InstantiateCategories();
                }
                //Populate the instantiated element with categories element
                for (int l = 0; l < test.Childs.Count(); l++)
                {
                    Categories category = new Categories();
                    category.id = test[l]["id"].AsInt;
                    category.name = test[l]["name"] == null?"":test[l]["name"].Value;
                    category.desc = test[l]["description"] == null?"":test[l]["description"].Value;
                    category.image = test[l]["image"] == null ? "" : test[l]["image"].Value;
                    catList.Add(category);
                    PopulateAndUpdateData(l);
                }
                //AppManager.Instnace.messageBoxManager.HidePreloader();
            }
            //When we already have elements in HUB
            else
            {
                //If there is some change in the categories count.
                if (test.Childs.Count() != catTransformList.Count())
                {
                    //Remove the categories object and Clear the previous Category list
                    for (int k = 0; k < catList.Count(); k++)
                    {
                        Destroy(catTransformList[k].gameObject);
                    }
                    catList.Clear();
                    catTransformList.Clear();

                    //Instantiate new categories
                    for (int j = 0; j < test.Childs.Count(); j++)
                    {
                        InstantiateCategories();
                    }
                    //Populate new categories with elements.
                    for (int b = 0; b < test.Childs.Count(); b++)
                    {
                        Categories category = new Categories();
                        category.id = test[b]["id"].AsInt;
                        category.name = test[b]["name"] == null ?"":test[b]["name"].Value;
                        category.desc = test[b]["description"] == null? "":test[b]["description"].Value;
                        category.image = test[b]["image"] == null?"":test[b]["image"].Value;
                        //category.desc = test[b]["description"].Value;
                        catList.Add(category);
                        PopulateAndUpdateData(b);
                    }
                } else {
                    catList.Clear();
                    //Populate new categories with elements.
                    for (int a = 0; a < test.Childs.Count(); a++)
                    {
                        Categories category = new Categories();
                        category.id = test[a]["id"].AsInt;
                        category.name = test[a]["name"] == null ?"":test[a]["name"].Value;
                        category.image = test[a]["image"] == null? "":test[a]["image"].Value;
                        category.desc = test[a]["description"] == null?"":test[a]["description"].Value;
                        catList.Add(category);
                        PopulateAndUpdateData(a);
                    }
                }
            }
        }
        AppManager.Instnace.messageBoxManager.HidePreloader();
        Invoke("AfterDelayOfSeconds", 1.0f);
    }

    void AfterDelayOfSeconds(){
        isContentLoaded = true;
    }

    void InstantiateCategories() {
        
        GameObject categoryItem = Instantiate(CategoryPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        //Image gImage = categoryItem.transform.Find("Panel/HistoryImage").GetComponent<Image>();
        categoryItem.transform.SetParent(parentObject);
        categoryItem.transform.localPosition = new Vector3(categoryItem.transform.position.x, categoryItem.transform.position.y, 0);
        categoryItem.transform.localScale = Vector3.one;
        catTransformList.Add(categoryItem.transform);
    }

    //Add or update data with this function
    void PopulateAndUpdateData(int m) {
         
        catTransformList[m].name = catList[m].id.ToString();
        Transform headTextTransform = catTransformList[m].GetChild(0);
        headTextTransform.name = catList[m].name;
        headTextTransform.GetComponent<Text>().text = catList[m].name;
        Transform descTransform = catTransformList[m].GetChild(1);
        descTransform.GetComponent<Text>().text = catList[m].desc;
        //Adding button listner
        catTransformList[m].GetComponentInChildren<Button>().onClick.AddListener(() => OnCategoryClicked(catTransformList[m]));
        //Adding category image
        string imagePath = string.IsNullOrEmpty(catList[m].image)?"": GetImageCachePath() + ReturnImageName(catList[m].image);
        if (File.Exists(imagePath))
        {
            byte[] byteArray = File.ReadAllBytes(imagePath);
            Texture2D texture = new Texture2D(128, 128);
            texture.LoadImage(byteArray);
            if (texture != null)
            {
                catTransformList[m].Find("Panel/HistoryImage").GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0, 0));
            }
        } else {
            if(!string.IsNullOrEmpty(catList[m].image)){
                StartCoroutine(LoadImage(catTransformList[m], catList[m].image));
            }
        }

        foreach (Transform trans in catTransformList)
        {
            trans.GetChild(4).gameObject.SetActive(false);
        }
    }

    IEnumerator LoadImage(Transform a_transform, string url)
    {
        WWW www = new WWW(url);
        yield return www;

        //a_transform.GetChild(2).GetComponentInChildren<Image>().sprite = Sprite.Create(www.texture, new Rect(0f, 0f, www.texture.width, www.texture.height), new Vector2(0, 0));
        a_transform.Find("Panel/HistoryImage").GetComponent<Image>().sprite = Sprite.Create(www.texture, new Rect(0f, 0f, www.texture.width, www.texture.height), new Vector2(0, 0));


        string imagePath = GetImageCachePath() + ReturnImageName(url);
        byte[] bytes = www.texture.EncodeToJPG();
        File.WriteAllBytes(imagePath, bytes);
    }

    public string GetImageCachePath()
    {
        string path = Application.persistentDataPath + "/HubCacheImage/";

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        return path;
    }
   
    string ReturnImageName(string imageUrl)
    {
        string[] breakApart = imageUrl.Split('/');
        string str = breakApart[breakApart.Length - 1];
        string[] breakDot = str.Split('.');
        string imageName = breakDot[0];

        return imageName;
    }
}


