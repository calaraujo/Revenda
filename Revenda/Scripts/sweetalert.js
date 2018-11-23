/*
 * This makes sure that we can use the global
 * swal() function, instead of swal.default()
 * See: https://github.com/webpack/webpack/issues/3929
 */

if (typeof window !== 'undefined') {
  require('./Scripts/sweetalert.css');
}

require('./Scripts/polyfills');

var swal = require('./Scripts/core').default;

module.exports = swal;
