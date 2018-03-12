using Ninject;
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebCrawler.Model;
using WebCrawler.Repository.Interface;
using WebCrawler.Service.Interface;

namespace WebCrawler
{
    internal partial class Main : Form, IObserver<Anchor>
    {
        private IWebCrawler _crawler;
        private IDisposable _queueSubscription;
        private IDisposable _queueListenerSubscription;
        private readonly IUriQueue _queue;

        private StandardKernel _kernel;
        //private readonly IUriQueueListener _queueListener;

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
        }

        public void OnNext(Anchor value)
        {
            lbAnchors.Invoke(new Action(() => lbAnchors.Items.Add(value.Uri)));
        }

        public void OnError(Exception error)
        {
            //
        }

        public void OnCompleted()
        {
            //
        }
    }
}
