using Ninject;
using System;
using System.Reflection;
using System.Windows.Forms;
using WebCrawler.Model;
using WebCrawler.Repository.Interface;
using WebCrawler.Service.Interface;

namespace WebCrawler
{
    internal partial class Main : Form, IObserver<Anchor>
    {
        private IDisposable _queueSubscription;
        private IDisposable _queueListenerSubscription;
        private readonly IUriQueue _queue;

        private readonly StandardKernel _kernel;

        internal Main()
        {
            _kernel = new StandardKernel();
            _kernel.Load(Assembly.GetExecutingAssembly());

            _queue = _kernel.Get<IUriQueue>();
            _queueSubscription = _queue.Subscribe(this);
            var queueListener = _kernel.Get<IUriQueueListener>();
            _queueListenerSubscription = _queue.Subscribe(queueListener);

            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            _queue.Enqueue(new Anchor { Uri = new Uri("http://www.thegravenimage.com/controltechnology") } );
            _queue.Enqueue(new Anchor { Uri = new Uri("http://www.appthem.com") } );

            var crawlFarm = _kernel.Get<ICrawlFarm>();
            crawlFarm.Run();

        }

        public void OnNext(Anchor anchor)
        {
            lbAnchors.Invoke(new Action(() => lbAnchors.Items.Add($"{anchor.JumpCount} : {anchor.Uri}")));
        }

        public void OnError(Exception error)
        {
            //
        }

        public void OnCompleted()
        {
            //
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
