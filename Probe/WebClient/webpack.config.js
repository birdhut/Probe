const path = require('path');
const webpack = require('webpack');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const ExtractTextPlugin = require('extract-text-webpack-plugin');

module.exports = {
    entry: [
        'react-hot-loader/patch',
        path.join(__dirname, 'WebClient/index.tsx')
    ],
    output: {
        path: path.join(__dirname, 'Service', 'Client'),
        filename: '[name].js',
        publicPath: '/'
    },
    plugins: [
        new webpack.HotModuleReplacementPlugin(),
        new webpack.NamedModulesPlugin(),
        new webpack.optimize.ModuleConcatenationPlugin(),
        new HtmlWebpackPlugin({
            filename: 'index.html',
            template: path.join(__dirname, 'WebClient', 'index.hbs')
        }),
        new ExtractTextPlugin("styles.css"),
    ],
    resolve: {
        extensions: ['.ts', '.tsx', '.js', '.json', '.css'],
        modules: [
            'node_modules',
            'WebClient',
        ]
    },
    devServer: {
        hot: true,
        inline: true,
    },
    module: {
        rules: [
            {
                test: /\.tsx?$/,
                use: ['react-hot-loader/webpack', 'awesome-typescript-loader'],
                exclude: /node_modules/,
            },
            {
                test: /\.(png|woff|woff2|eot|ttf|svg)$/,
                loader: 'url-loader?limit=100000'
            },
            {
                test: /\.css$/, 
                use: ['style-loader', 'css-loader']
            }
        ]
    },
    devtool: 'source-map'
};
