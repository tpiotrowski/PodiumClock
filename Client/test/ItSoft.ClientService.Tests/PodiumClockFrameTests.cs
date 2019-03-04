using System;
using System.Collections.Generic;
using System.Text;
using ItSoft.ClientService;
using NUnit.Framework;

namespace Tests
{
    //[TestFixture]
    public class PodiumClockFrameTests
    {
        [SetUp]
        public void Setup()
        {
        }


        static (BaseMessage Source, ClockAndTextMessage Result) PrepareFrameClockTextPF(string mm, string ss,
            string text, bool indicator,
            char signMarker)
        {
            List<byte> type = new List<byte>();
            StringTooBytes(type, "T1");


            List<byte> body = new List<byte>();

            PrepareFrameBody(mm, ss, text, indicator, signMarker, body);

            return (new BaseMessage()
            {
                Body = body.ToArray(),
                Type = type.ToArray()
            }, new ClockAndTextMessage()
            {
                Minutes = mm,
                Seconds = ss,
                Text = text,
                IndicatorEnabled = indicator,
                Sign = signMarker,
            });
        }

        static byte[] PrepareFrameClockText(string mm, string ss, string text, bool indicator, char signMarker)
        {
            List<byte> bytes = new List<byte> {0x11};


            StringTooBytes(bytes, "T1");

            PrepareFrameBody(mm, ss, text, indicator, signMarker, bytes);

            bytes.Add(0x13);

            return bytes.ToArray();
        }

        private static void PrepareFrameBody(string mm, string ss, string text, bool indicator, char signMarker,
            List<byte> bytes)
        {
            bytes.Add(Convert.ToByte(signMarker));

            StringTooBytes(bytes, mm);

            bytes.Add(Convert.ToByte(':'));

            StringTooBytes(bytes, ss);

            bytes.Add(Convert.ToByte(':'));

            bytes.Add(indicator ? (byte)'1' : (byte) '0');

            bytes.Add(Convert.ToByte(':'));

            bytes.Add(0x02);

            if (!string.IsNullOrEmpty(text))
            {
                StringTooBytes(bytes, text);
            }

            bytes.Add(0x03);
        }

        private static void StringTooBytes(List<byte> list, string innerText)
        {
            list.AddRange(Encoding.UTF8.GetBytes(innerText));
        }


        static object[] FrameSource =
        {
            new object[] {PrepareFrameClockText("13", "23", "TestText", false, ' ')},
        };

        [Test(), TestCaseSource(nameof(FrameSource))]
        public void PodiumClockFrameDecodeTest(byte[] frame)
        {
            var podiumClockFrame = BaseMessage.Decode(frame);


            Assert.Multiple(() =>
            {
                Assert.That(podiumClockFrame, Is.Not.Null);

                Assert.That(podiumClockFrame.Type, Is.Not.Null);
                Assert.That(podiumClockFrame.Type, Is.Not.Empty);

                Assert.That(podiumClockFrame.Type.Length, Is.EqualTo(2));
                Assert.That(podiumClockFrame.Type, Is.EqualTo(Encoding.UTF8.GetBytes("T1")));


                Assert.That(podiumClockFrame.Body, Is.Not.Null);
                Assert.That(podiumClockFrame.Body, Is.Not.Empty);
            });
        }


        [Test()]
        public void PodiumClockTimeAndTextFrameDecodeTest()
        {
            var testData = PrepareFrameClockTextPF("13", "23", "TestText", false, ' ');

           
            var clockAndTimeFrame = new ClockAndTimeMessageDecoder().Decode(testData.Source.Type, testData.Source.Body);

            Assert.Multiple(() =>
            {
                Assert.That(clockAndTimeFrame, Is.Not.Null);

                Assert.That(clockAndTimeFrame.Minutes, Is.EqualTo(testData.Result.Minutes),"Check minutes");
                Assert.That(clockAndTimeFrame.Seconds, Is.EqualTo(testData.Result.Seconds), "Check seconds");
                Assert.That(clockAndTimeFrame.Text, Is.EqualTo(testData.Result.Text),"Check text");
                Assert.That(clockAndTimeFrame.IndicatorEnabled, Is.EqualTo(testData.Result.IndicatorEnabled), "Check indicator");
                Assert.That(clockAndTimeFrame.Sign, Is.EqualTo(testData.Result.Sign), "Check sign");
            });
        }

        [Test()]
        public void PodiumClockTimeAndTextFrameDecodeTest2()
        {
            var testData = PrepareFrameClockTextPF("13", "23", "Test:Text", false, ' ');


            var clockAndTimeFrame = new ClockAndTimeMessageDecoder().Decode(testData.Source.Type, testData.Source.Body);

            Assert.Multiple(() =>
            {
                Assert.That(clockAndTimeFrame, Is.Not.Null);

                Assert.That(clockAndTimeFrame.Minutes, Is.EqualTo(testData.Result.Minutes), "Check minutes");
                Assert.That(clockAndTimeFrame.Seconds, Is.EqualTo(testData.Result.Seconds), "Check seconds");
                Assert.That(clockAndTimeFrame.Text, Is.EqualTo(testData.Result.Text), "Check text");
                Assert.That(clockAndTimeFrame.IndicatorEnabled, Is.EqualTo(testData.Result.IndicatorEnabled), "Check indicator");
                Assert.That(clockAndTimeFrame.Sign, Is.EqualTo(testData.Result.Sign), "Check sign");
            });
        }

        [Test()]
        public void PodiumClockTimeAndTextFrameDecodeTest3()
        {
            var testData = PrepareFrameClockTextPF("13", "23", "Test:Text", true, '-');


            var clockAndTimeFrame = new ClockAndTimeMessageDecoder().Decode(testData.Source.Type, testData.Source.Body);

            Assert.Multiple(() =>
            {
                Assert.That(clockAndTimeFrame, Is.Not.Null);

                Assert.That(clockAndTimeFrame.Minutes, Is.EqualTo(testData.Result.Minutes), "Check minutes");
                Assert.That(clockAndTimeFrame.Seconds, Is.EqualTo(testData.Result.Seconds), "Check seconds");
                Assert.That(clockAndTimeFrame.Text, Is.EqualTo(testData.Result.Text), "Check text");
                Assert.That(clockAndTimeFrame.IndicatorEnabled, Is.EqualTo(testData.Result.IndicatorEnabled), "Check indicator");
                Assert.That(clockAndTimeFrame.Sign, Is.EqualTo(testData.Result.Sign), "Check sign");
            });
        }

        [Test()]
        public void PodiumClockTimeAndTextFrameDecodeTest4()
        {
            var testData = PrepareFrameClockTextPF("13", "23", "Test:& T ext", true, '-');


            var clockAndTimeFrame = new ClockAndTimeMessageDecoder().Decode(testData.Source.Type, testData.Source.Body);

            Assert.Multiple(() =>
            {
                Assert.That(clockAndTimeFrame, Is.Not.Null);

                Assert.That(clockAndTimeFrame.Minutes, Is.EqualTo(testData.Result.Minutes), "Check minutes");
                Assert.That(clockAndTimeFrame.Seconds, Is.EqualTo(testData.Result.Seconds), "Check seconds");
                Assert.That(clockAndTimeFrame.Text, Is.EqualTo(testData.Result.Text), "Check text");
                Assert.That(clockAndTimeFrame.IndicatorEnabled, Is.EqualTo(testData.Result.IndicatorEnabled), "Check indicator");
                Assert.That(clockAndTimeFrame.Sign, Is.EqualTo(testData.Result.Sign), "Check sign");
            });
        }
    }
}