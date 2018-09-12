using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RxLab
{
    class Program
    {
        static void Main(string[] args)
        {
            GetAllAsObservable()
                .ObserveOn(new EventLoopScheduler()) // 処理の結果を特定のスレッド上のイベントループで処理するように指定して
                .Subscribe(x =>
                {
                    // 結果を表示
                    Console.WriteLine($"{x}, printed on thread{Thread.CurrentThread.ManagedThreadId}");
                },
                ex => Console.WriteLine(ex.Message),   // 例外時の処理
                () => Console.WriteLine("Completed")); // 完了時の処理
            Console.ReadLine();
        }

        // バラバラのスレッドから値を発行する処理のダミー
        static IObservable<string> GetAllAsObservable()
        {
            return Observable.Create<string>(async ox =>
            {
                var r = new Random();
                await Task.WhenAll(
                    PublishValueAfter(ox, "0", 1000 + r.Next(5000)),
                    PublishValueAfter(ox, "1", 1000 + r.Next(5000)),
                    PublishValueAfter(ox, "2", 1000 + r.Next(5000)),
                    PublishValueAfter(ox, "3", 1000 + r.Next(5000))
                );
            });
        }

        static async Task PublishValueAfter(IObserver<string> ox, string value, int delay)
        {
            await Task.Delay(delay).ConfigureAwait(false);
            ox.OnNext($"{value} from thread{Thread.CurrentThread.ManagedThreadId}");
        }
    }
}
