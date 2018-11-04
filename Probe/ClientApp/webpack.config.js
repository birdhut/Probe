const path = require('path');
const webpack = require('webpack');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const UglifyJsPlugin = require("uglifyjs-webpack-plugin");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const OptimizeCSSAssetsPlugin = require("optimize-css-assets-webpack-plugin");

module.exports = (env, argv) => {
    const inDir = path.resolve(__dirname, 'src');
    const outputDir = path.resolve(__dirname, 'dist');
    const devMode = argv.mode !== 'production'
    console.info(`Dev Mode: ${devMode} (mode=${argv.mode})`);
    const plugins = [];
    if (!devMode) {
        plugins.push(new webpack.DefinePlugin({ "process.env.NODE_ENV": JSON.stringify("production") }));
    }
    plugins.push(new HtmlWebpackPlugin({
        filename: 'index.html',
        template: path.join(inDir, 'index.hbs'),
        useMockData: devMode ? true : false,
    }));
     plugins.push(new MiniCssExtractPlugin({
            // Options similar to the same options in webpackOptions.output
            // both options are optional
            filename: devMode ? '[name].css' : '[name].[hash:6].css',
            chunkFilename: devMode ? '[name].css' : '[name].[hash:6].css',
    }));

    return {
        entry: {
            probeweb: path.join(inDir, 'index.tsx'),
        },
        module: {
            rules: [
            {
                test: /\.tsx?$/,
                use: 'ts-loader',
                exclude: /node_modules/
            },
            {
                test: /\.css$/,
                use: [
                    devMode ? 'style-loader' : MiniCssExtractPlugin.loader,
                    'css-loader'
                ]
            }
            ]
        },
        optimization: {
            minimizer: [
            new UglifyJsPlugin({
                cache: true,
                parallel: true,
                sourceMap: true // set to true if you want JS source maps
            }),
            new OptimizeCSSAssetsPlugin({})
            ]
        },
        resolve: {
            extensions: [ '.tsx', '.ts', '.js' ]
        },
        output: {
            filename: devMode ? '[name].js' : '[name].[hash:6].js',
            path: outputDir
        },
        plugins
    };
};
