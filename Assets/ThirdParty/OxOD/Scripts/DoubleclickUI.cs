using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace OxOD
{
    public class DoubleclickUI : MonoBehaviour, IPointerClickHandler
    {
        int tap;
        public float interval = 0.5f;
        bool readyForDoubleTap = false;
        public UnityEvent onSingleClick;
        public UnityEvent onDoubleClick;

        public void OnPointerClick(PointerEventData eventData)
        {
            tap++;

            if (tap == 1)
            {
                onSingleClick.Invoke();
                StartCoroutine(DoubleTapInterval());
            }

            else if (tap > 1 && readyForDoubleTap)
            {
                onDoubleClick.Invoke();
                StopCoroutine(DoubleTapInterval());
                tap = 0;
                readyForDoubleTap = false;
            }
        }

        IEnumerator DoubleTapInterval()
        {
            readyForDoubleTap = true;
            yield return new WaitForSeconds(interval);
            readyForDoubleTap = false;
            tap = 0;
        }
    }
}