const masks = {
  cpf: (value) => value
    .replace(/\D/g, '')
    .slice(0, 11)
    .replace(/(\d{3})(\d)/, '$1.$2')
    .replace(/(\d{3})\.(\d{3})(\d)/, '$1.$2.$3')
    .replace(/(\d{3})\.(\d{3})\.(\d{3})(\d)/, '$1.$2.$3-$4'),
  cep: (value) => value
    .replace(/\D/g, '')
    .slice(0, 8)
    .replace(/(\d{5})(\d)/, '$1-$2'),
  digits: (value) => value.replace(/\D/g, ''),
  numero: (value) => value.replace(/[^0-9A-Za-z/-]/g, '').toUpperCase(),
  placa: (value) => {
    const chars = value.replace(/[^0-9A-Za-z]/g, '').toUpperCase().slice(0, 7);
    return /^[A-Z]{3}\d{2,4}$/.test(chars)
      ? chars.slice(0, 3) + '-' + chars.slice(3)
      : chars;
  },
};

document.querySelectorAll('[data-mask]').forEach((input) => {
  const mask = masks[input.dataset.mask];
  if (!mask) {
    return;
  }
  const apply = () => {
    input.value = mask(input.value);
  };
  input.addEventListener('input', apply);
  apply();
});

const toastSources = document.querySelectorAll('[data-toast]');
if (toastSources.length) {
  const region = document.createElement('div');
  region.className = 'toast-region';
  document.body.appendChild(region);

  const icons = {
    success: '<svg viewBox="0 0 256 256" width="20" height="20" fill="currentColor" aria-hidden="true"><path d="M173.66,98.34a8,8,0,0,1,0,11.32l-56,56a8,8,0,0,1-11.32,0l-24-24a8,8,0,0,1,11.32-11.32L112,148.69l50.34-50.35A8,8,0,0,1,173.66,98.34ZM232,128A104,104,0,1,1,128,24,104.11,104.11,0,0,1,232,128Zm-16,0a88,88,0,1,0-88,88A88.1,88.1,0,0,0,216,128Z"/></svg>',
    danger: '<svg viewBox="0 0 256 256" width="20" height="20" fill="currentColor" aria-hidden="true"><path d="M128,24A104,104,0,1,0,232,128,104.11,104.11,0,0,0,128,24Zm0,192a88,88,0,1,1,88-88A88.1,88.1,0,0,1,128,216Zm-8-80V80a8,8,0,0,1,16,0v56a8,8,0,0,1-16,0Zm20,36a12,12,0,1,1-12-12A12,12,0,0,1,140,172Z"/></svg>',
  };

  toastSources.forEach((source) => {
    const toast = document.createElement('div');
    toast.className = `toast toast-${source.dataset.toast}`;
    toast.setAttribute('role', 'status');

    const icon = document.createElement('span');
    icon.className = 'toast-icon';
    icon.innerHTML = icons[source.dataset.toast] ?? icons.danger;
    toast.appendChild(icon);

    const message = document.createElement('span');
    message.className = 'toast-message';
    message.textContent = source.textContent.trim();
    toast.appendChild(message);

    const dismiss = document.createElement('button');
    dismiss.type = 'button';
    dismiss.className = 'toast-dismiss';
    dismiss.setAttribute('aria-label', 'Fechar');
    dismiss.textContent = '×';
    toast.appendChild(dismiss);

    let hideTimer;
    const hide = () => {
      clearTimeout(hideTimer);
      toast.classList.add('toast-leaving');
      setTimeout(() => toast.remove(), 500);
    };
    dismiss.addEventListener('click', hide);
    region.appendChild(toast);
    hideTimer = setTimeout(hide, 3200);
  });
}

document.querySelectorAll('[data-viacep]').forEach((cepInput) => {
  const form = cepInput.form;
  const field = (name) => form.querySelector(`[name$=".${name}"]`);
  let lastLookup = '';

  cepInput.addEventListener('input', async () => {
    const digits = cepInput.value.replace(/\D/g, '');
    if (digits.length !== 8 || digits === lastLookup) {
      return;
    }
    lastLookup = digits;
    try {
      const response = await fetch(`https://viacep.com.br/ws/${digits}/json/`);
      if (!response.ok) {
        return;
      }
      const data = await response.json();
      if (data.erro) {
        return;
      }
      const fill = (name, value) => {
        const input = field(name);
        if (input && value) {
          input.value = value;
        }
      };
      fill('Logradouro', data.logradouro);
      fill('Bairro', data.bairro);
      fill('Cidade', data.localidade);
      fill('Uf', data.uf);
      field('Numero')?.focus();
    } catch {
      lastLookup = '';
    }
  });
});

document.querySelectorAll('[data-combo]').forEach((combo) => {
  const input = combo.querySelector('input');
  const list = combo.querySelector('.combo-list');
  const options = [...list.querySelectorAll('[role="option"]')];
  const normalize = (text) =>
    text.normalize('NFD').replace(/[\u0300-\u036f]/g, '').toUpperCase();

  let activeIndex = -1;

  const visible = () => options.filter((opt) => !opt.hidden);

  const setActive = (index) => {
    const opts = visible();
    activeIndex = Math.max(0, Math.min(index, opts.length - 1));
    options.forEach((opt) => opt.classList.remove('active'));
    if (opts[activeIndex]) {
      opts[activeIndex].classList.add('active');
      opts[activeIndex].scrollIntoView({ block: 'nearest' });
    }
  };

  const open = () => {
    const term = normalize(input.value.trim());
    options.forEach((opt) => {
      opt.hidden = Boolean(term)
        && !opt.dataset.value.startsWith(term)
        && !normalize(opt.textContent).includes(term);
    });
    const any = visible().length > 0;
    list.hidden = !any;
    input.setAttribute('aria-expanded', String(any));
    setActive(0);
  };

  const close = () => {
    list.hidden = true;
    input.setAttribute('aria-expanded', 'false');
    activeIndex = -1;
  };

  const pick = (option) => {
    input.value = option.dataset.value;
    close();
  };

  input.addEventListener('focus', open);
  input.addEventListener('input', open);
  input.addEventListener('blur', close);
  input.addEventListener('keydown', (event) => {
    if (list.hidden) {
      return;
    }
    if (event.key === 'ArrowDown') {
      event.preventDefault();
      setActive(activeIndex + 1);
    } else if (event.key === 'ArrowUp') {
      event.preventDefault();
      setActive(activeIndex - 1);
    } else if (event.key === 'Enter') {
      const option = visible()[activeIndex];
      if (option) {
        event.preventDefault();
        pick(option);
      }
    } else if (event.key === 'Escape') {
      close();
    }
  });

  options.forEach((option) => {
    option.addEventListener('mousedown', (event) => {
      event.preventDefault();
      pick(option);
    });
  });
});
