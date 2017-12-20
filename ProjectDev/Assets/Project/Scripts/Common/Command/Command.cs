namespace Common.Command
{
    public delegate void OnCommandSuccess(Command command,object content);
    public delegate void OnCommandFail(Command command, object content);
    public delegate void OnCommandCancel(Command command, object content);


    public class Command
    {
        public event OnCommandSuccess onSuccess;
        public event OnCommandFail onFail;
        public event OnCommandCancel onCancel;

        protected object _content;

        public virtual void Execute(object content)
        {
            this._content = content;
        }

        public virtual void Cancel()
        {
            Cancel(this._content);
        }

        protected void Success(object content)
        {
            if (this.onSuccess != null)
            {
                this.onSuccess(this,content);
            }
        }

        protected void Fail(object content)
        {
            if (this.onFail != null)
            {
                this.onFail(this, content);
            }
        }

        protected void Cancel(object content)
        {
            if (this.onCancel != null)
            {
                this.onCancel(this, content);
            }
        }
    }
}