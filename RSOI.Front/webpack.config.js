var path = require('path');
var webpack = require('webpack');

module.exports = {
  devtool: 'eval',
  mode: 'development',
  entry: [
    './src/index'
  ],
  output: {
    path: path.join(__dirname, 'dist'),
    filename: 'bundle.js',
    publicPath: '/static/'
  },
  plugins: [
    new webpack.HotModuleReplacementPlugin()
  ],
  resolve: {
    extensions: ['.js', '.jsx']
  },
  module: {
    rules: [{
      test: /\.js(x)?$/,
      use: ['babel-loader'],
      include: path.join(__dirname, 'src')
    },{
        test: [/\.css$/],
        use: [ 'style-loader', 'css-loader' ]
    },{
        test: /.(ttf|otf|eot|svg|woff(2)?)(\?[a-z0-9]+)?$/,
        use: {
            loader: "file-loader",
            options: {
                name: "fonts/[name].[ext]"
            }
        }
    }]
  }
};
