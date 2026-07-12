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
