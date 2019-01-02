using System;
using System.Collections.Generic;
using System.Text;
using ItSoft.ClientService;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class PodiumClockFrameTests
    {
        [SetUp]
        public void Setup()
        {
        }


        static PodiumClockFrame PrepareFrameClockTextPF(string mm, string ss, string text, bool indicator,
            char signMarker)
        {
            List<byte> type = new List<byte>();
            StringTooBytes(type, "T1");


            List<byte> body = new List<byte>();

            PrepareFrameBody(mm, ss, text, indicator, signMarker, body);

            return new PodiumClockFrame()
            {
                Body = body.ToArray(),
                Type = type.ToArray()
            };
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

            bytes.Add(indicator ? (byte) 1 : (byte) 0);

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
            var podiumClockFrame = PodiumClockFrame.Decode(frame);


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


        static object[] FrameSourcePF =
        {
            new object[]
            {
                PrepareFrameClockTextPF("13", "23", "TestText", false, ' '), new ClockAndTimeFrame()
                {
                    Minutes = "13",
                    Seconds = "23",
                    Text = "TestText",
                    IndicatorEnabled = false,
                    Sign = ' ',
                    

                }
            },
        };

        [Test, TestCaseSource(nameof(FrameSourcePF))]
        public void PodiumClockTimeAndTextFrameDecodeTest(PodiumClockFrame podiumClockFrame, ClockAndTimeFrame result)
        {
            var clockAndTimeFrame = ClockAndTimeFrame.Decode(podiumClockFrame);

            Assert.Multiple(() =>
            {
                Assert.That(clockAndTimeFrame, Is.Not.Null);

                Assert.That(clockAndTimeFrame.Minutes, Is.EqualTo(result.Minutes));
                Assert.That(clockAndTimeFrame.Seconds, Is.EqualTo(result.Seconds));
                Assert.That(clockAndTimeFrame.Text, Is.EqualTo(result.Text));
                Assert.That(clockAndTimeFrame.IndicatorEnabled, Is.EqualTo(result.IndicatorEnabled));
                Assert.That(clockAndTimeFrame.Sign, Is.EqualTo(result.Sign));
            });
        }
    }
}