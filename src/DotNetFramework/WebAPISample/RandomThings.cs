using System;
using System.Collections.Generic;
using System.Threading;

namespace WebAPISample
{
    public static class RandomThings
    {
        const int MIN = 50;
        const int MIDDLE = 200;
        const int LARGE = 1000;

        static List<Type> exceptions = new List<Type> {
            typeof(Exception),
            typeof(ArgumentException),
            typeof(NullReferenceException),
            typeof(TimeoutException),
            typeof(IndexOutOfRangeException)
        };

        /// <summary>
        /// 随机等待，用于模拟程序处理时的延迟。
        /// </summary>
        /// <param name="speed">处理速度。默认：50~1000ms、慢速：200~1000ms、快速：50~200ms。</param>
        public static void RandomSleep(Speed speed)
        {
            int time;

            switch (speed)
            {
                case Speed.High:
                    time = new Random().Next(MIN, MIDDLE);
                    break;

                case Speed.Slow:
                    time = new Random().Next(MIDDLE, LARGE);
                    break;

                default:
                    time = new Random().Next(MIN, LARGE);
                    break;
            }

            Thread.Sleep(time);
        }

        /// <summary>
        /// 随机异常，用于模拟程序处理时抛出异常的情况。
        /// </summary>
        public static void RandomCrash()
        {
            var random = new Random();

            if (random.Next(0, 10) < 1)
            {
                var index = random.Next(0, exceptions.Count);
                var exceptionType = exceptions[index];

                var exception = exceptionType.Assembly.CreateInstance(exceptionType.FullName) as Exception;

                throw exception;
            }
        }
    }

    public enum Speed
    {
        Default,
        High,
        Slow
    }
}