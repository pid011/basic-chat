using BasicChat.Lib;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        ChatPacket packet = new ChatPacket
        {
            Status = ChatStatus.Welcome,
            Name = "Test"
        };

        // ChatPacket.Send(client, packet);
    }

    // Update is called once per frame
    private void Update()
    {

    }
}
