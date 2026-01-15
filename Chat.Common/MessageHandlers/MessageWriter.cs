using Newtonsoft.Json;
using System.Buffers.Binary;
using System.Text;




namespace Chat.Common.MessageHandlers;


public class MessageWriter(Stream stream) : MessageHandler, IDisposable
{
    public async Task WriteMessage(MessageDTO message, CancellationToken ct)
    {
        string output = JsonConvert.SerializeObject(message);

        byte[] bytes = Encoding.UTF8.GetBytes(output);
        int bytes_length = bytes.Length;
        if (bytes_length > MaxMessageLen) throw new TooLongMessageException("Too long  message");


        byte[] header = new byte[4];
        BinaryPrimitives.WriteInt32BigEndian(header, bytes_length);

        await stream.WriteAsync(header, 0, 4, ct);
        
        await stream.WriteAsync(bytes, 0, bytes_length, ct);
    }


    public void Dispose()
    {
        stream.Dispose();
    }
}
