using UnityEngine;

namespace Fungus.Demos
{
    public class ListenForSayDialogDemoTest : MonoBehaviour
    {
        void OnEnable()
        {
            ListenForEvents();
        }

        protected virtual void ListenForEvents()
        {
            SayDialogSignals.Started += OnSayDialogStart;
            SayDialogSignals.Ended += OnSayDialogEnd;
        }

        protected virtual void OnSayDialogStart(SayDialogEventArgs args)
        {
            Debug.Log("Say dialog started doing something!\n" + args.ToString());

        }

        protected virtual void OnSayDialogEnd(SayDialogEventArgs args)
        {
            Debug.Log("Say dialog finished doing something!\n" + args.ToString());
        }
    }
}