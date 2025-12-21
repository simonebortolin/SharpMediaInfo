using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SharpMediaInfo.Native.Interop
{
    internal sealed class MediaInfoHandle : SafeHandle
    {
        public MediaInfoHandle() : base(IntPtr.Zero, true)
        {
            SetHandle(MediaInfoNativeMethods.CreateHandle());
        }

        public override bool IsInvalid => handle == IntPtr.Zero;

        protected override bool ReleaseHandle()
        {
            MediaInfoNativeMethods.DeleteHandle(handle);
            return true;
        }
    }
}
