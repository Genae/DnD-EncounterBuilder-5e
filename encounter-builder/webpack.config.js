﻿var webpack = require('webpack');
var path = require('path');
var HtmlWebpackPlugin = require('html-webpack-plugin');
var ExtractTextPlugin = require("extract-text-webpack-plugin");

module.exports = {
    mode: 'development',
    entry: path.resolve(__dirname, 'src/main.ts'),
    devServer: {
        proxy: {
            '/api': {
                target: 'http://localhost:63995',
                secure: false
            }
        }
    },
    output: {
        path: path.resolve(__dirname, 'wwwroot'),
        filename: 'app.[hash].js'
    },
    module: {
        rules: [
            { test: /\.component.ts$/, loaders: 'angular2-template-loader' },
            { test: /\.ts$/, loaders: 'awesome-typescript-loader' },
            { test: /\.html$/, loaders: 'html-loader' },
            { test: /\.css$/, loaders: 'css-loader' },
            { test: /\.css$/, loaders: ExtractTextPlugin.extract({ fallback: "style-loader", use: "css-loader" }) },
            { test: /\.(png|svg|jpg|gif)$/, use: [ 'file-loader'] }
        ]
    },
    resolve: {
        extensions: ['*', '.js', '.ts', '.html', '.css']
    },
    devtool: 'source-map',
    plugins: [
        new HtmlWebpackPlugin({
            template: './src/index.html'
        })
    ]
};