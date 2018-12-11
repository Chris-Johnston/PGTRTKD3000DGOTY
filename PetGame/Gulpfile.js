/// <binding BeforeBuild='default' AfterBuild='default' Clean='clean' ProjectOpened='default' />
/*
This file is the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require('gulp');
var del = require('del');
var less = require('gulp-less');
var debug = require('gulp-debug');

var paths = {
    scripts: ['scripts/**/*.js', 'scripts/**/*.ts', 'scripts/**/*.map'],
};

gulp.task('cleanscripts', function () {
    return del(['wwwroot/scripts/**/*']);
});
gulp.task('cleanless', function () {
    return del(['wwwroot/css/**/*']);
});

gulp.task('clean', gulp.parallel(['cleanscripts', 'cleanless']));

gulp.task('scripts', function () {
    // log the cwd
    console.log("running in path " + __filename);
    return gulp.src(paths.scripts)
        .pipe(gulp.dest('wwwroot/scripts'))
        .pipe(debug({ title: 'scripts output:' }));
});

gulp.task('less', function () {
    return gulp.src('style/main.less')
        .pipe(less())
        .pipe(gulp.dest('wwwroot/css'))
        .pipe(debug({ title: 'less output:' }));
});

gulp.task('default', gulp.series(['clean', 'scripts', 'less']));