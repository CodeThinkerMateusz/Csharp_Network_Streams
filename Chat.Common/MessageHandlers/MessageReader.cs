using System.Buffers.Binary;
using System.Text;
using Newtonsoft.Json;


namespace Chat.Common.MessageHandlers;


public class MessageReader(Stream stream) : MessageHandler, IDisposable
{
    public async Task<MessageDTO?> ReadMessage(CancellationToken ct)
    {
        try
        {
            byte[] header = new byte[4];
            await stream.ReadExactlyAsync(header, ct);

            int bytes_read = BinaryPrimitives.ReadInt32BigEndian(header);
            if (bytes_read > MaxMessageLen) throw new TooLongMessageException("Too big message /skeleton head emoji/");

            byte[] message = new byte[bytes_read];

            await stream.ReadExactlyAsync(message, ct);

            string output = Encoding.UTF8.GetString(message);
            
            var final = JsonConvert.DeserializeObject<MessageDTO>(output);
            return final;
        }
        catch(EndOfStreamException)
        {
            return null;
        }
        catch (JsonSerializationException)
        {
            throw new InvalidMessageException("JSON failed");
        }
    }


    public void Dispose()
    {
        stream.Dispose();
    }
}