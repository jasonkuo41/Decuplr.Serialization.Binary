using System;
using System.Collections.Generic;
using System.Text;

namespace Decuplr.CodeAnalysis {
	internal ref struct SpanWriter<T> {

		private readonly Span<T> _buffer;

		public int Written { get; private set; }
		public ReadOnlySpan<T> Current => _buffer.Slice(0, Written);

		public SpanWriter(Span<T> buffer) {
			_buffer = buffer;
			Written = 0;
		}

		public void Write(ReadOnlySpan<T> span) {
			span.CopyTo(_buffer.Slice(Written));
			Written += span.Length;
		}

		public void Write(T element) {
			_buffer[Written] = element;
			Written++;
		}
	}
}
