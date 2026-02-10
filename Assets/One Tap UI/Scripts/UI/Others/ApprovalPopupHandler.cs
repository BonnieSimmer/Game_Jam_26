using System;
using UnityEngine.UIElements;

namespace One_Tap_UI.UI.Others
{
    [Serializable]
    public class ApprovalPopupHandler
    {
        public UIDocument document;
        private VisualElement root;
        private Action lastOnApprove, lastOnDecline;
        
        /// <summary>
        /// <para> Setup the approval popup. </para>
        /// </summary>
        /// <param name="message"> The message that will be shown. </param>
        /// <param name="approve"> Text of the button that calls the positive action. </param>
        /// <param name="decline"> Text of the button that calls the negative action. </param>
        public void Setup(string message, string approve, string decline)
        {
            document.gameObject.SetActive(true);
            root = document.rootVisualElement.Q<VisualElement>("ApprovalPopup");
            root.Q<Label>("Message").text = message;
            root.Q<Button>("Positive").text = approve;
            root.Q<Button>("Negative").text = decline;
            root.visible = false;
        }
        
        /// <summary>
        /// <para> Shows the approval popup. </para>
        /// </summary>
        /// <param name="onApprove"> Action that will be called when the positive/approve button is clicked. </param>
        /// <param name="onDecline"> Action that will be called when the negative/decline button is clicked. </param>
        public void Show(Action onApprove, Action onDecline)
        {
            var positive = root.Q<Button>("Positive");
            var negative = root.Q<Button>("Negative");
            positive.clickable.clicked -= lastOnApprove;
            lastOnApprove = onApprove;
            lastOnApprove += () => root.visible = false;
            positive.clickable.clicked += lastOnApprove;
            negative.clickable.clicked -= lastOnDecline;
            lastOnDecline = onDecline;
            lastOnDecline += () => root.visible = false;
            negative.clickable.clicked += lastOnDecline;
            root.visible = true;
        }
    }
}