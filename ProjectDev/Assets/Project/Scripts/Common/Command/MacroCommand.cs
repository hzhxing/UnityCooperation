using System.Collections.Generic;

namespace Common.Command
{
    public class MacroCommand:Command
    {
        private List<Command> _listCommand = new List<Command>();
        private int _curIndex = 0;

        public MacroCommand()
        {
            InitCommand();
        }

        protected virtual void InitCommand()
        {
            
        }

        protected void AddSubCommand(Command command)
        {
            this._listCommand.Add(command);
        }

        public override void Execute(object content)
        {
            base.Execute(content);
            this._curIndex = 0;

            ExecuteCommand();
        }

        public override void Cancel()
        {
            if (this._curIndex < this._listCommand.Count)
            {
                Command command = this._listCommand[this._curIndex];
                RemoveSubCommandCallback(command);
                base.Cancel(this._content);
            }
        }

        private void ExecuteNextCommand()
        {
            this._curIndex++;
            if (this._curIndex >= this._listCommand.Count)
            {
                this.Success(this._content);
                return;
            }
            ExecuteCommand();
        }

        private void ExecuteCommand()
        {
            Command command = this._listCommand[this._curIndex];
            command.onSuccess += OnSubCommandSuccess;
            command.onCancel += OnSucCommandCancel;
            command.onFail += OnSubCommandFail;
            command.Execute(this._content);
        }


        private void RemoveSubCommandCallback(Command command)
        {
            command.onSuccess -= OnSubCommandSuccess;
            command.onCancel -= OnSucCommandCancel;
            command.onFail -= OnSubCommandFail;
        }

        private void OnSubCommandSuccess(Command command, object content)
        {
            this._content = content;
            RemoveSubCommandCallback(command);
            ExecuteNextCommand();
        }

        private void OnSubCommandFail(Command command, object content)
        {
            RemoveSubCommandCallback(command);
            Fail(content);
        }

        private void OnSucCommandCancel(Command command, object content)
        {
            RemoveSubCommandCallback(command);
            Cancel(content);
        }
    }
}