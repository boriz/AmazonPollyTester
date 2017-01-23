using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using NAudio;
using System.Threading;
using System.IO;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Count() == 0)
            {
                Console.WriteLine("Basic text to speach converter");
                Console.WriteLine("Expecting text in the command line parameter");
                Environment.Exit(0);
            }

            Console.WriteLine("Creating Amazon Polly client");
            Amazon.Polly.AmazonPollyClient cl = new Amazon.Polly.AmazonPollyClient(RegionEndpoint.USWest2);
            Amazon.Polly.Model.SynthesizeSpeechRequest req = new Amazon.Polly.Model.SynthesizeSpeechRequest();
            req.Text = String.Join(" ", args);
            req.VoiceId = Amazon.Polly.VoiceId.Salli;
            req.OutputFormat = Amazon.Polly.OutputFormat.Mp3;
            req.SampleRate = "8000";
            req.TextType = Amazon.Polly.TextType.Text;

            Console.WriteLine("Sending Amazon Polly request: " + req.Text);
            Amazon.Polly.Model.SynthesizeSpeechResponse resp = cl.SynthesizeSpeech(req);

            MemoryStream local_stream = new MemoryStream();
            resp.AudioStream.CopyTo(local_stream);            
            local_stream.Position = 0;
            Console.WriteLine("Got mp3 stream, lenght: " + local_stream.Length.ToString());

            NAudio.Wave.Mp3FileReader reader = new NAudio.Wave.Mp3FileReader(local_stream);
            NAudio.Wave.WaveStream wave_stream = NAudio.Wave.WaveFormatConversionStream.CreatePcmStream(reader);
            NAudio.Wave.BlockAlignReductionStream ba_stream = new NAudio.Wave.BlockAlignReductionStream(wave_stream);
            NAudio.Wave.WaveOut wout = new NAudio.Wave.WaveOut();

            Console.Write("Playing stream...");
            wout.Init(ba_stream);
            wout.Play();
            while (wout.PlaybackState == NAudio.Wave.PlaybackState.Playing)
            {
                Thread.Sleep(100);
            }
            Console.WriteLine("..Done");
        }
    }
}
