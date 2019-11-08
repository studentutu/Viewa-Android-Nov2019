using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CategoryClickHandler : MonoBehaviour {

    public IOnClickCategory iOnClickCategory;

    public void HeartClick() {
        iOnClickCategory.OnHeartClickHandler(this.GetComponent<RectTransform>());
    }

    public void CategoryClick(){
        iOnClickCategory.OnCategoryClickHandler(this.GetComponent<RectTransform>());
    }


    public interface IOnClickCategory
    {
        void OnHeartClickHandler(RectTransform transform);
        void OnCategoryClickHandler(RectTransform transform);
    }

}
